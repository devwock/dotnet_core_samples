# ShortcutMaker

C#에서 단축 아이콘을 만들어주는 코드입니다.

## CreateShortcutByClsid

Windows Script Host의 [WshShortcut Object](https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/windows-scripting/xk6kst2k(v=vs.84))를 이용하여 단축 아이콘을 만듭니다.

dynamic을 쓰지 않은 이유는 닷넷 3.5 이하 호환성 및 닷넷 코어 3.0 기준으로 `Activator.CreateInstance(type);` 실행 시 `null`이 나오기 때문입니다.

## CreateShortcutByWsh

Windows Script Host의 [WshShortcut Object](https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/windows-scripting/xk6kst2k(v=vs.84))를 이용하여 단축 아이콘을 만듭니다.

`Interop.IWshRuntimeLibrary` 참조가 필요합니다.

## CreateShortcutByCom

[IShellLinkA interface](https://docs.microsoft.com/en-us/windows/win32/api/shobjidl_core/nn-shobjidl_core-ishelllinka)를 이용하여 단축 아이콘을 만듭니다.
