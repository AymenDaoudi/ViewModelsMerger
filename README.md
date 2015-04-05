# ![ViewModelsMerger](https://raw.githubusercontent.com/AymenDaoudi/ViewModelsMerger/ReadMe/Icons/ViwModelMergerIcon64.png)    ViewModelsMerger 

## 1. What is ViewModelsMerger ?

ViewModelsMerger is a library for WPF, that helps you merge different ViewModels into one ViewModel at runtime.

## 2. Why Merge different ViewModels into one ?

In cases where a View can have elements, controls or different regions that have different ViewModels, You don't need to change
the DataContext in code every time for each part of the view, you just set one DataContext for the top parrent object that 
encompasses the different DataContexts (ViewModels) and Merge them into one ViewModel at RunTime.

## 3. Usage :

### Before :

When using a ViewModel for each part of the view :

![Take a snip] (https://raw.githubusercontent.com/AymenDaoudi/ViewModelsMerger/ReadMe/ReadMeScreenShots/Before.png)

### After :
When Merging the ViewModels using ViewModelsMerger, which offers a Converter and by using MultyBinding :

![Take a snip] (https://raw.githubusercontent.com/AymenDaoudi/ViewModelsMerger/ReadMe/ReadMeScreenShots/After.png)

### Result :

![Take a snip] (https://github.com/AymenDaoudi/ViewModelsMerger/blob/ReadMe/ReadMeScreenShots/BeforeAfterResult.png)

### Special cases

There are some special scenarios different ViewModels can have properties with the same name, in that case you just precise which 
ViewModel the property belongs to by using an index :

*ViewModel 1*

![Take a snip] (https://raw.githubusercontent.com/AymenDaoudi/ViewModelsMerger/ReadMe/ReadMeScreenShots/ViewModel1.png)

*ViewModel 2*

![Take a snip] (https://raw.githubusercontent.com/AymenDaoudi/ViewModelsMerger/ReadMe/ReadMeScreenShots/ViewModel2.png)


*XAML*

![Take a snip] (https://raw.githubusercontent.com/AymenDaoudi/ViewModelsMerger/ReadMe/ReadMeScreenShots/CommonProperties.png)

*Result*

![Take a snip] (https://raw.githubusercontent.com/AymenDaoudi/ViewModelsMerger/ReadMe/ReadMeScreenShots/Result2.png)

## 4. Installing :

## 5. Caution :

The ViewModelsMerger relays on the [MVVMLight] (https://mvvmlight.codeplex.com/#summary) library from which it uses the class ViewModelBase.

## License :

This project is under the [Creative Commons Attribution NonCommercial NoDerivs (CC-NC-ND)] (https://tldrlegal.com/license/creative-commons-attribution-noncommercial-noderivs-(cc-nc-nd)#summary) fully described in the (c.f. [License.txt](License.txt)) file.

![CC-NC-ND](http://i.creativecommons.org/l/by-nc-nd/3.0/88x31.png)
