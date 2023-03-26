Maniac Framework (for Unity project)
--
## Features:
- DataBase
- UI
- Profile (Save/Load)
- Spawner
- LanguageTable (Localization)
- Audio
- Messenger
- Time
- Other helper Utils

## Here is how to get into your project.
Guide:
- Step 1: Clone this repo as a submodule into your Unity project. You can put it in

![img.png](ImagesForReadme/img.png)

- Step 2: There will be a lot of errors. I know, **DON'T PANIC** ☠. Follow this sub steps.
  - Step 2.1: Install [Dotween](http://dotween.demigiant.com/getstarted.php).
  - Step 2.2: Install [Unitask](https://github.com/Cysharp/UniTask/releases).
  - Step 2.3: Install [UniRx](https://github.com/neuecc/UniRx/releases).
  - Dotween is for UI animation transitions.
  - Unitask is for async await programming
  - UniRx is for using Observer pattern.
  
- Step 3: Hopefully at this point. There will be no errors.

- Step 4: Open Bootstrap scene in Maniac/Bootstrap

 ![img.png](ImagesForReadme/img_1.png)

- Step 5: You need to create some scriptable objects for Bootstrap script 

 ![img_1.png](ImagesForReadme/img_2.png)

- Step 6: Just follow these. You'll be fine. 

 ![img.png](ImagesForReadme/img_3.png)

- Step 7: After followed 4 steps. You will have 4 scriptable object which located at Assets/Resources/ 

 ![img.png](ImagesForReadme/img_4.png)

- Step 8: Put all those scriptable objects into Bootstrap.

- Step 9: Run the Scene. If there is no error. You have completed implemented Maniac Framework into your project.

 ![img.png](ImagesForReadme/img_5.png)

# Note: use Locator<>.Instance as Singleton 

## DataBase System
### Storing your game data as a scriptable object and get it during runtime with ease.

- Create your own scriptable object in second. (I'll prefer to call it xxxConfig)

 ![img.png](img.png)

- All configs(and their scripts) will be stored at Assets/Recources/DatabaseConfigs

 ![img_1.png](img_1.png)![img_2.png](img_2.png)

- You can custom any data of any kind into your script and edit it in editor.

![img_3.png](img_3.png)

- You can get it any where during runtime using example like this.

![img_4.png](img_4.png)

## UI System
### Create UI base in seconds. Edit it however you want. Show and Close it with ease.

- Make some UI Layers. This will help you later when you need some UIs to always be on top of some others 

![img_6.png](img_6.png)

- Create your new UI.

![img_5.png](img_5.png)
 
- Choose your desired transition, layer, basic setting

![img_7.png](img_7.png)

- Show and Close UI example

![img_8.png](img_8.png)

- There are more methods in BaseUI.cs class. Take a look at it. 

## Profile (Save/Load) System
### create a Json Save/Load system with ease

- Make a class which store data that need to be saved , and inherit from ProfileRecord

![img_9.png](img_9.png)

- You can check your saved files here !

![img_10.png](img_10.png)

## Spawner (Object Pooling) System
### Object Pool using Unity ObjectPool<T>

- Create any prefab that you want to clone, and clone it like example
- **Notice**
- After Changing Scene to Scene, you need to reset all of the spawner

![img_11.png](img_11.png)

## LanguageTable (Localization) System
### Localization system for all type of texts

- Create languages that you want to have in your project

![img_12.png](img_12.png)

- Create Language Item for any text that you want to be localized.

![img_13.png](img_13.png)

- Open Language Table and edit the language item that you just created

![img_14.png](img_14.png)

- You can make a TMP_Text, automatically change to other language using LanguageText component. Remember to Add the LanguageItem for it :P

![img_15.png](img_15.png)

- Sometime during runtime, you may need to use the LanguageItem in script. Here is how to do it.

![img_16.png](img_16.png)

- Small Tips. Use "_" to make LanguageItem goes into its own section

![img_17.png](img_17.png)

## Audio System
### audio for 2d games, it could be use in 3d games, but you need to read down into the code a bit

- Step by Step
- Create new Audio Info ,  Add AudioClip into variations

![img_18.png](img_18.png)

- During runtime when you need to play that sound. This is how to do it in script

![img_19.png](img_19.png)