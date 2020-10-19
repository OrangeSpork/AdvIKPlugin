# AdvIKPlugin

Requirement: KKAPI 1.13.2(+) -> https://github.com/IllusionMods/IllusionModdingAPI
Requirement: ExtensibleSaveFormat -> https://github.com/IllusionMods/BepisPlugins

This plugin adds additional advanced control over the IK solver and positioning of the final poses.

Current Features: IK Shoulder Rotation Control, IK Spine Hints from FK, Procedural Breathing Animation Overlay

Shoulder Rotation Control

Tells the IK Solver to move the Shoulder to help achieve IK reach goals. If you don't know what that means, it means that when the arms are reaching up or down, the shoulder will now actually rotate to make that look more natural.

To use:

Click on character.
Click Anim->Kinematics->Adv IK

Enable/Disable Shoulder Rotation Correction\
Shoulder Weight controls how strongly the shoulder rotates\
Shoulder Offset controls how far the hands have to move before the rotation starts to kick in\
Spine Stiffness (works regardless of Shoulder Rotation) Controls how much the IK solver bends the spine during shoulder/hips movement. Higher number means less spine movement.\

IK Spine Hints from FK

Passes the current FK rotation of the Spine01 and Spine02 bones into the IK solver to weight it towards using that spine curvature.

To use:

Click on character.
Click Anim->Kinematics->FK and enable Body FK
Rotate Spine1 and Spine2 to achieve the desired spine curvature
Turn off Body FK
Enable Body IK
Click Anim->Kinematics->Adv IK

Enable Spine FK Hints

Position body IK as desired

Note: Enabling Shoulder Rotation is strongly recommended, the extra solver path available helps the hint apply as desired greatly. Set Shoulder Weight and Offset to 0 if you don't want shoulder movement, but leave the rotation toggle on.

Procedural Breathing

This is a procedurally animated breathing overlay. It simulates breathing by actively adjusting the scale of the chest in an appropriate (and configurable) manner. This is an animation overlay, meaning it is procedurally computed from the current final state pose...and then removed prior to the next animation pass. Thus it is compatible with IK, FK, other animations and just about anything else.

To use:

Click Anim->Kinematics->Adv IK->Breath

Set Enable Breathing to active

Settings available:

Breath Size %: A quick slider to scale up or down the size of the chest expansion. Larger number exagerates the motion, smaller makes it...smaller.\
Intake Pause %: Percentage of intake time spent holding at minimum inhalation. Basically how long the char waits with empty breath before starting the inhale.\
Hold Pause %: Percentage of exhale time spent holding at maximum inhalation. How long the char holds the breath at the 'top' of the breathing cycle.\
Inhale %: Percentage of breath time spent inhaling vs. exhaling. For example 0.6 means that 60% of breath time is inhale, 40% exhale.\
Breath Per Min: Number of breaths taken per minute. Normal human respiration is 12-18 BPM at rest, increasing to 30-40 when exercising. 50+ is hyperventilation territory.\
Shldr Damp %: Shoulder dampening percentage. Reduces the shoulder movement caused by the chest movement. At 1 the shoulders do not move at all (and thus arm position doesn't fluctuate). At 0 the shoulders move naturally with the chest. Values in the middle split the difference.\

Restore Default: Sets everything back to factory shine.

Advanced Shape Options:

Overall Breath Scale:  Allows quick scaling of all three components (Upper Chest, Lower Chest, Abdomen) in both X, Y and Z scales.\
Upper Chest Scale: Upper chest scaling applied at maximum inhalation.\
Lower Chest Scale: Lower chest scaling applied at maximum inhalation.\
Abdomen Scaling: Abdomen scaling applied at maximum inhalation. Note, use a negative number for diaphragm breathing, positive for belly breathing.\

Note: The individual component scaling is multiplied by the Overall Breath Scale and then by the Breath Size %...so watch the multiplicative effects :)
