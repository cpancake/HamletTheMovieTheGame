using UnityEngine;
using System.Collections;
using System.Linq;

public class IntroWalk : MonoBehaviour
{
	public Animator Anim;
	public AudioClip[] Footsteps;
	public AudioSource Source;
	public float Interval = 1;
	public float PauseTime = 60;
	public GameObject OldCam;
	public GameObject NewCam;
	public GameObject Spot;
	public GameObject TheLight;
	public GameObject MainCam;
	public AudioClip SpotEffect;
	public AudioSource HamletAudio;
	public AudioSource NoHamletAudio;

	private bool _end = false;
	private float _counter = 0;
	private float _standCounter = 0;
	private float _pauseCounter = 0;

	void Update()
	{
		if(_pauseCounter > PauseTime) return;

		if(_end)
		{
			if(_standCounter > 1f)
			{
				_pauseCounter += Time.deltaTime;
				if(_pauseCounter > PauseTime)
				{
					OldCam.GetComponent<Camera>().enabled = false;
					NewCam.GetComponent<Camera>().enabled = true;
					Spot.GetComponent<Light>().enabled = true;
					HamletAudio.enabled = true;
					NoHamletAudio.enabled = true;
				}
				return;
			}
			else
			{
				_standCounter += Time.deltaTime;
				if(_standCounter > 1f)
				{
					MainCam.GetComponent<KeyGUI>().enabled = true;
					Spot.GetComponent<Light>().enabled = false;
					TheLight.GetComponent<Light>().enabled = false;
					Source.PlayOneShot(SpotEffect);
				}
				return;
			}
		}

        transform.localPosition -= new Vector3(0, 0, 2f * Time.deltaTime);
		if(transform.localPosition.z <= 3f)
		{
			Anim.SetBool("done_walking", true);
			_end = true;
			_standCounter += Time.deltaTime;
		}

		if(_counter > Interval)
		{
			_counter = 0;
			var clip = Footsteps[(int)Mathf.Floor(Random.value * Footsteps.Length)];
			Source.PlayOneShot(clip);
		}

		_counter += Time.deltaTime;
	}
}