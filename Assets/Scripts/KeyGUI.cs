using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using UnityStandardAssets.ImageEffects;

public class KeyGUI : MonoBehaviour {
	public Texture[] KeyImages;
	public Texture[] DarkKeyImages;
	public Texture NoteImage;
	public int SizeDivisor = 2;
	public float Speed = 1;
	public int NoteHeight = 20;
	public TextAsset Cues;
	public AudioSource HamletAudio;
	public AudioSource NoHamletAudio;
	public AudioSource SfxSource;
	public AudioClip HitClip;
	public Camera[] Cameras = new Camera[4];
	public System.Type[] Effects = new System.Type[] {
		typeof(EdgeDetection),
		typeof(Blur),
		typeof(NoiseAndScratches),
		typeof(Grayscale),
		typeof(Fisheye)
	};

	private bool _started = false;
	private bool[] _keysDown = new bool[4];
	private bool[] _keysNewlyDown = new bool[4];
	private List<double> _cues;
	private List<int> _cueLanes = new List<int>();
	private bool _thingHit = false;
	private int _numHit = 0;
	private int _totalNotes = 0;
	private int _score = 0;
	private float _guiStartY = 100;
	private float _camTimer = 0;
	private int _camNumber = 0;

	public void Awake()
	{
		var lines = Cues.text.Split('\n');
		_cues = new List<double>(lines.Select(x => ParseLine(x)).OrderBy(x=>x));
		_totalNotes = _cues.Count;
	}

	public void Start()
	{
		_guiStartY = Screen.height;
		//Speed = Speed * (750 / Screen.height);
	}

	private double ParseLine(string line)
	{
		var parts = line.Split(' ');
		_cueLanes.Add(int.Parse(parts[0]));
		var timeParts = parts[1].Split(':');
		if(timeParts.Length == 1)
			return double.Parse(timeParts[0]);
		var minutes = double.Parse(timeParts[0]);
		return (minutes * 60) + double.Parse(timeParts[1]);
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

		if(!HamletAudio.isPlaying && _started)
		{
			this.enabled = false;
			var end = this.gameObject.AddComponent<EndScript>();
			end.Score = _score;
			end.TotalNotes = _totalNotes;
			end.NumHit = _numHit;
			end.Score = _score;
			return;
		}

		_keysDown[0] = Input.GetKey(KeyCode.Q);
		_keysDown[1] = Input.GetKey(KeyCode.W);
		_keysDown[2] = Input.GetKey(KeyCode.E);
		_keysDown[3] = Input.GetKey(KeyCode.R);
		_keysNewlyDown[0] = Input.GetKeyDown(KeyCode.Q);
		_keysNewlyDown[1] = Input.GetKeyDown(KeyCode.W);
		_keysNewlyDown[2] = Input.GetKeyDown(KeyCode.E);
		_keysNewlyDown[3] = Input.GetKeyDown(KeyCode.R);
		if(!_thingHit && _keysNewlyDown.Any(x => x))
		{
			HamletAudio.mute = true;
			NoHamletAudio.mute = false;
			SfxSource.PlayOneShot(HitClip);
			_score -= 50;
		}
		if(_thingHit)
		{
			HamletAudio.mute = false;
            NoHamletAudio.mute = true;
			_score += 100;
			_numHit += 1;
		}
		_camTimer += Time.deltaTime;
		if(_camTimer >= 10f)
		{
			_camNumber++;

			_camTimer = 0;
			var theChosenCam = (int)Mathf.Floor(Random.value * Cameras.Length);
			for(int i = 0; i < Cameras.Length; i++)
			{
				var cam = Cameras[i];
				cam.enabled = (i == theChosenCam);

				if(cam.enabled && _camNumber > 4)
				{
					var theChosenEffect = (int)Mathf.Floor(Random.value * Effects.Length);
					for(int j = 0; j < Effects.Length; j++)
					{
						if(cam.GetComponent(Effects[j]) == null)
						{
							if(Effects[j] == typeof(EdgeDetection))
							{
								var edge = cam.gameObject.AddComponent<EdgeDetection>();
								edge.edgesOnly = Random.value;
							}
							else if(Effects[j] == typeof(NoiseAndScratches))
							{
								var noise = cam.gameObject.AddComponent<NoiseAndScratches>();
								noise.grainSize = Random.value * 10;
								noise.scratchIntensityMin = Random.value * 10;
							}
							else if(Effects[j] == typeof(Grayscale))
							{
								cam.gameObject.AddComponent<Grayscale>();
							}
							else if(Effects[j] == typeof(Fisheye))
							{
								var fish = cam.gameObject.AddComponent<Fisheye>();
								fish.strengthX = Random.value * 5;
								fish.strengthY = Random.value * 5;
							}
						}
						((MonoBehaviour)cam.gameObject.GetComponent(Effects[j])).enabled = (j == theChosenEffect);
					}
				}
			}
		}
		_thingHit = false;
    }
    
    public void OnGUI()
	{
		if(_guiStartY > 100)
		{
			_guiStartY -= (Time.deltaTime * 100);
		}
		GUI.Label(new Rect(0, 0, 100, 100), "Score: " + _score);
        int width = KeyImages.Sum(x => x.width / SizeDivisor);
		int startX = Screen.width / 2 - width / 2;
		int currentX = startX;
		for(int i = 0; i < KeyImages.Length; i++)
		{
			GUI.Box(
				new Rect(
					currentX,
					_guiStartY,
					KeyImages[i].width / SizeDivisor,
					Screen.height - 100
				),
				"hey"
			);
			GUI.DrawTexture(
				new Rect(
					currentX,
					_guiStartY,
					KeyImages[i].width / SizeDivisor,
					KeyImages[i].height / SizeDivisor
				),
				_keysDown[i] ? DarkKeyImages[i] : KeyImages[i]
			);
			currentX += KeyImages[i].width / SizeDivisor;
		}

		float currentTime = HamletAudio.time;
		int keyHeight = KeyImages[0].height / SizeDivisor;
		int startY = 100 + (keyHeight / 2) - (NoteHeight / 2);
		int height = Screen.height - startY;
	restart:
		for(var i = 0; i < _cues.Count; i++)
		{
			double timeDiff = _cues[i] - currentTime;
			if(timeDiff > 5) 
				break;
			double yPos = startY + (timeDiff / (5 * (1 / Speed))) * height;
			if(yPos > Screen.height)
				break;
			if(yPos < 100 - NoteHeight)
			{
				_cues.RemoveAt(i);
				_cueLanes.RemoveAt(i);
				NoHamletAudio.mute = false;
				_score -= 100;
				HamletAudio.mute = true;
				SfxSource.PlayOneShot(HitClip);
				goto restart;
			}
			_started = true;
			if(!_thingHit && yPos >= 100 && yPos <= 100 + keyHeight)
			{
				if(
					_cueLanes[i] == 1 && Input.GetKeyDown(KeyCode.Q) ||
					_cueLanes[i] == 2 && Input.GetKeyDown(KeyCode.W) ||
					_cueLanes[i] == 3 && Input.GetKeyDown(KeyCode.E) ||
					_cueLanes[i] == 4 && Input.GetKeyDown(KeyCode.R)
				)
				{
					_cues.RemoveAt(i);
					_cueLanes.RemoveAt(i);
					_thingHit = true;
					goto restart;
				}
			}
			GUI.DrawTexture(
				new Rect(
					startX + (_cueLanes[i] - 1) * (KeyImages[0].width / SizeDivisor),
					(float)yPos,
					50,
					NoteHeight
				),
				NoteImage
			);
		}
	}
}
