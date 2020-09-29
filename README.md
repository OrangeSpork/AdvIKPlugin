# AdvIKPlugin

Requirement: KKAPI 1.13.2(+) -> https://github.com/IllusionMods/IllusionModdingAPI
Requirement: ExtensibleSaveFormat -> https://github.com/IllusionMods/BepisPlugins

This plugin enables additional logic around deforming the shoulder in hopefully realistic ways during IK solving the arm positions.

To use:

Click on character.
Click Anim->Kinematics->Adv IK

Enable/Disable Shoulder Rotation Correction\
Shoulder Weight controls how strongly the shoulder rotates\
Shoulder Offset controls how far the hands have to move before the rotation starts to kick in\
Spine Stiffness (works regardless of Shoulder Rotation) Controls how much the IK solver bends the spine during shoulder/hips movement. Higher number means less spine movement.\
