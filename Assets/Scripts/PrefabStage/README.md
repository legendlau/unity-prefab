# PrefabStage Examples

This directory contains examples demonstrating Unity's PrefabStage functionality, which is used for editing prefabs in isolation mode.

## Features Demonstrated

1. **Prefab Stage Events**
   - Opening prefab stage (`OnPrefabStageOpened`)
   - Closing prefab stage (`OnPrefabStageClosing`)
   - Scene saving handling (`OnSceneSaving`)

2. **Stage Information**
   - Getting current prefab stage
   - Accessing prefab root
   - Checking prefab paths
   - Determining prefab variant status

3. **Prefab Validation**
   - Component validation
   - Name uniqueness checks
   - Custom validation rules

4. **Editor Integration**
   - Menu items for common operations
   - Opening prefabs in isolation
   - Checking modifications

## Usage

The examples can be accessed through the Unity Editor menu under "Examples/PrefabStage/":
- Check Current Stage
- Open Prefab
- Check Modifications

The script also automatically hooks into Unity's prefab workflow events to provide logging and validation.