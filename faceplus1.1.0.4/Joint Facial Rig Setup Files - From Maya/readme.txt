MIXAMO_FacePlus_SDK_Export
written by Dan Babcock

Exports SDK data into JSON to use in Unity with Face Plus
Requires a single node that has Mixamo Face Plus output names as attributes, hooked up to the facial joints through set driven keys
An FBX containing an empty transform node with properly named attributes is included in the zip

TO USE:
    -In Maya, open the scene with your joint-based facial rig
    -Import in FacePlusTransformNode.fbx
    -create set driven keys for the channels you want Face Plus to control
	*If you already have a node with all the SDKs on it, just make sure the attributes are named according to FacePlus standards
        (Animation Tab -> Animate -> Set Driven Key -> Set)
        see the Maya documentation for more on Set Driven Keys
    -run the script
        -point it to a folder to export to and the Facial_Hookup node that you put the SDKs on
        -Hit Export
    -In Unity, make sure your character has the Face Plus Connector script applied
        -drop in your Head and Eye joint transforms into the appropriate places
        -change the Type to Joint_SDK
        -the exported file (will be called SDK_Preset.txt) can be dropped into the SDK Preset slot
        -you will need to populate the Face Joints list with the joints referenced in the SDK Preset file
    


MORE INFORMATION:

    Joint Based Rigs for FacePlus require a way to map values from the plugin to deltas for moving the joints.
    The current implementation in the example Unity FacePlus scene needs a JSON file describing:
        the delta (change) in translation (labeled as "dtx","dty","dtz"),
        rotation (labeled as "drx","dry","drz"), 
        and scale (labeled as "dsx","dsy","dsz"), 
        for each joint on the face rig for each FacePlus channel
    Usable channels are:
        MouthOpen
        MouthUp
        MouthDown
        Smile_Left
        Smile_Right
        Frown_Left
        Frown_Right
        UpperLipUp_Left
        UpperLipUp_Right
        LowerLipDown_Left
        LowerLipDown_Right
        MouthNarrow_Left
        MouthNarrow_Right
        MouthWhistle_NarrowAdjust_Left
        MouthWhistle_NarrowAdjust_Right
        Squint_Left
        Squint_Right
        EyesWide_Left
        EyesWide_Right
        Blink_Left
        Blink_Right
        NoseScrunch_Left
        NoseScrunch_Right
        BrowsDown_Left
        BrowsDown_Right
        BrowsUp_Left
        BrowsUp_Right
        BrowsIn_Left
        BrowsIn_Right
        BrowsOuterLower_Left
        BrowsOuterLower_Right
        Midmouth_Left
        Midmouth_Right
        UpperLipIn
        UpperLipOut
        LowerLipIn
        LowerLipOut
        CheekPuffLeft
        CheekPuffRight
        TongueUp
        JawUp
        JawDown
        JawLeft
        JawRight
        JawIn
        JawOut
        JawRotYPos
        JawRotYNeg
        JawRotZPos
        JawRotZNeg
        
    The organization is as follows (whitespace added for legibility):
    
    {
        "FacePlusChannel2": 
        {
            "joint2": 
            {
                "dtx": 0.81260239754446828
            }, 
            "joint1": 
            {
                "dtx": 0.81260239754446828
            }
        }, 
        "FacePlusChannel": 
        {
            "joint2": {}, 
            "joint1": 
            {
                "drx": 0.068981552327341433, 
                "dtx": -1.2831292072035581
            }
        }
    }