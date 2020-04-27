# Shutdown

.net core 앱에서 전원 관리를 할 수 있는 코드입니다.

## 원리

WMI의 Win32_OperatingSystem 클래스의 Win32ShutdownTracker 메소드를 사용하여 시스템의 전원을 관리하는 코드입니다.

## 모드

* 로그오프: LogOff
* 강제 로그오프: ForcedLogOff
* 셧다운: Shutdown
* 강제 셧다운: ForcedShutdown,
* 재부팅: Reboot
* 강제 재부팅: ForcedReboot
* 전원 끄기: PowerOff
* 강제 전원 끄기: ForcedPowerOff

## 참조

* <https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32shutdowntracker-method-in-class-win32-operatingsystem>
