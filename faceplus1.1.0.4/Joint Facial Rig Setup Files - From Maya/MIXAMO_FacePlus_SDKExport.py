"""
MIXAMO_FacePlus_SDK_Export
written by Dan Babcock

Exports SDK data into JSON to use in Unity with Face Plus
Requires a single node that has Mixamo Face Plus output names as attributes, hooked up to the facial joints through set driven keys
An FBX containing an empty transform node with properly named attributes is included in the zip

TO USE:
    -Open the scene with your joint-based facial rig
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
"""

import maya.cmds as mayac
import maya.mel as mel
import os, json


def unique(my_list): 
    return list(set(my_list))

def findConnectedJoints(node):
    connectedJoints = []
    connectedAll = mayac.listConnections(node, d=True, s=False)
    for con in connectedAll:
        if mayac.objectType(con) == "joint":
            connectedJoints.append(str(con))
        else:
            childConnections = findConnectedJoints(con)
            if childConnections:
                connectedJoints.append(childConnections[0])
    return unique(connectedJoints)

def buildDictionary(sdkNode):
    #lists of starting values
    stx = []
    sty = []
    stz = []
    srx = []
    sry = []
    srz = []
    ssx = []
    ssy = []
    ssz = []
    xforms = findConnectedJoints(sdkNode)
    for xform in xforms:
        print xform
    
    sdkList = mayac.listAttr(sdkNode, k=True, l=False, ud=True)
    #zero out sdks to get correct starting values
    for attr in sdkList:
        mayac.setAttr("%s.%s"%(sdkNode,attr),0)
    for x in xforms:
        stx.append(mayac.getAttr("%s.tx"%(x)))
        sty.append(mayac.getAttr("%s.ty"%(x)))
        stz.append(mayac.getAttr("%s.tz"%(x)))
        srx.append(mayac.getAttr("%s.rx"%(x)))
        sry.append(mayac.getAttr("%s.ry"%(x)))
        srz.append(mayac.getAttr("%s.rz"%(x)))
        ssx.append(mayac.getAttr("%s.sx"%(x)))
        ssy.append(mayac.getAttr("%s.sy"%(x)))
        ssz.append(mayac.getAttr("%s.sz"%(x)))
    sdks = {}
    for sdk in sdkList:
        joints = {}
        for j in range(len(xforms)):
            data = {}
            #gather deltas
            mayac.setAttr("%s.%s"%(sdkNode,sdk),1)
            dtx = mayac.getAttr("%s.tx"%(xforms[j])) - stx[j]
            dty = mayac.getAttr("%s.ty"%(xforms[j])) - sty[j]
            dtz = mayac.getAttr("%s.tz"%(xforms[j])) - stz[j]
            drx = mayac.getAttr("%s.rx"%(xforms[j])) - srx[j]
            dry = mayac.getAttr("%s.ry"%(xforms[j])) - sry[j]
            drz = mayac.getAttr("%s.rz"%(xforms[j])) - srz[j]
            dsx = mayac.getAttr("%s.sx"%(xforms[j])) - ssx[j]
            dsy = mayac.getAttr("%s.sy"%(xforms[j])) - ssy[j]
            dsz = mayac.getAttr("%s.sz"%(xforms[j])) - ssz[j]
            #unity scale is .01
            dtx /= -100
            dty /= 100
            dtz /= 100
            dry *= -1
            drz *= -1
            mayac.setAttr("%s.%s"%(sdkNode,sdk),0)
            #build libraries
            if dtx != 0:
                data.setdefault("dtx",dtx)
            if dty != 0:
                data.setdefault("dty",dty)
            if dtz != 0:
                data.setdefault("dtz",dtz)
            if drx != 0:
                data.setdefault("drx",drx)
            if dry != 0:
                data.setdefault("dry",dry)
            if drz != 0:
                data.setdefault("drz",drz)
            if dsx != 0:
                data.setdefault("dsx",dsx)
            if dsy != 0:
                data.setdefault("dsy",dsy)
            if dsz != 0:
                data.setdefault("dsz",dsz)
            joints.setdefault(str(xforms[j]), data)
        sdks.setdefault(str(sdk), joints)
    return sdks

def exportJSON(dict, path):
    f=open(os.path.join(path,"SDK_Preset.txt"),'w')
    f.write(json.dumps(dict, sort_keys=True, indent=2, separators=(',', ': ')))
    f.close()

def DJB_BrowserWindow(filter_ = None, caption_ = "Browse", fileMode_ = "directory"):
    multipleFilters = None
    filtersOld = None
    if filter_ == "Maya":
        multipleFilters = "Maya Files (*.ma *.mb);;Maya ASCII (*.ma);;Maya Binary (*.mb)"
        filtersOld = None
    elif filter_ == "Maya_FBX":
        multipleFilters = "Maya Files (*.ma *.mb);;Maya ASCII (*.ma);;Maya Binary (*.mb);;FBX (*.fbx);;All Files (*.*)"
    elif filter_ == "FBX":
        multipleFilters = "FBX (*.fbx);;All Files (*.*)"
    else:
        multipleFilters = ""
    window = None    
    version = mel.eval("float $ver = `getApplicationVersionAsFloat`;")
    if version <= 2011.0:
        if fileMode_ == "directory":
            window = mayac.fileBrowserDialog(dialogStyle = 2, windowTitle = caption_, fileType = "directory")
    else: #new style dialog window
        if fileMode_ == "directory":
            window = mayac.fileDialog2(fileFilter=multipleFilters, dialogStyle=2, caption = caption_, fileMode = 3, okCaption = "Select")
        else:
            window = mayac.fileDialog2(fileFilter=multipleFilters, dialogStyle=2, caption = caption_, fileMode = 4, okCaption = "Select")
    if window:
        return window[0]
    else:
        return window

class MIXAMO_SDK_Export_UI:
    def __init__(self):
        self.file1Dir = None
        self.name = "MIXAMO SDK Exporter"
        self.title = "MIXAMO SDK Exporter"

        # Begin creating the UI
        if (mayac.window(self.name, q=1, exists=1)): 
            mayac.deleteUI(self.name)
        self.window = mayac.window(self.name, title=self.title, menuBar=True)
        
        #forms
        self.form = mayac.formLayout(w=650)
        mayac.columnLayout(adjustableColumn = True, w=650)

        mayac.text( label='', align='left' )
        self.sdkNode_textFieldButtonGrp = mayac.textFieldButtonGrp( label='Node with SDKs', text='', buttonLabel='Load from Selection', buttonCommand = self.loadNode_function)
        
        mayac.separator( height=40, style='in' )
        self.exportPath_textFieldButtonGrp = mayac.textFieldButtonGrp( label='SDK File Save Path', text='', buttonLabel='Browse', buttonCommand = self.exportPath_function)
        mayac.text( label='', align='left' )
        self.exportButton = mayac.button(label = "Export", w=15, c=self.export_function)
        mayac.text( label='', align='left' )
        
        mayac.window(self.window, e=1, w=650, h=515, sizeable = 1) #580,560
        mayac.showWindow(self.window)
        
        
    def loadNode_function(self, arg = None): 
        sel = mayac.ls(sl=True)
        if (sel):
            mayac.textFieldButtonGrp(self.sdkNode_textFieldButtonGrp, edit = True,  text = sel[0])
        else:
            mayac.error("You must have the sdk transform node selected.")
        
             
    def export_function(self, arg = None):
        savePath = mayac.textFieldButtonGrp(self.exportPath_textFieldButtonGrp, query = True,  text = True)
        if savePath:
            node = mayac.textFieldButtonGrp(self.sdkNode_textFieldButtonGrp, query = True,  text = True)
            if node:
                dict = buildDictionary(node)
                exportJSON(dict,savePath)
            else:
                mayac.error("You must have the sdk transform node loaded.")
        else:
            mayac.error("You must choose a path for the output file.")
        
    def exportPath_function(self, arg = None):
        savePath = DJB_BrowserWindow(filter_ = None, caption_ = "Browse for save folder", fileMode_ = "directory")
        if savePath:
            mayac.textFieldButtonGrp(self.exportPath_textFieldButtonGrp, edit = True, text = savePath)
        else:
            mayac.textFieldButtonGrp(self.exportPath_textFieldButtonGrp, edit = True, text = "")

MIXAMO_SDK_Export_UI()