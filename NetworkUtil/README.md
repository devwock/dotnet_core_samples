# NetworkUtil

네트워크에 대한 여러가지 유틸을 모아둔 프로젝트입니다.

네트워크의 정보를 확인하는 방법은 상당히 여러가지가 있으며 테스트용으로 몇가지 추렸습니다.

## Network

### GetActiveNetwork

활성화 된 네트워크 정보를 반환합니다.

### GetDnsAddress

DNS 주소를 반환합니다.

### GetGatewayAddress

게이트웨이 주소를 반환합니다.

### GetIpAddress

IP 주소를 반환합니다.

### GetSubnetMask

서브넷 마스크를 반환합니다.

### GetNicSpeed

NIC의 스펙 상 속도를 반환합니다. 실제 인터넷 속도가 아닙니다.

### GetMacAddressByArp

APR 리퀘스트로 확인한 맥 주소를 반환합니다.

### GetMacAddressByManagementObject

WMI로 확인한 맥 주소를 반환합니다.

### GetMacAddressBySpeed

NIC의 속도로 확인한 맥 주소를 반환합니다.

### GetMacAddressByUnicast

유니캐스트 주소로 확인한 맥 주소를 반환합니다.

### SetDns

DNS 주소를 설정합니다.

### SetGateway

게이트웨이 주소를 설정합니다.

### SetIpAddress

IP, 서브넷 마스크를 설정합니다.

### SetNetwork

IP, 서브넷 마스크, 게이트웨이, DNS를 설정합니다.

## StringUtil

## SplitString

문자열을 특정 숫자 단위로 잘라 특정 문자열을 삽입합니다.  
주로 MAC 주소에 `:` 문자를 넣을 때 사용합니다.

`string.Join(":", StringUtil.SplitString(MAC주소, 2)));` 로 사용하면 됩니다.

### SplitInParts

`SplitString`의 래퍼입니다.  

## SubnetUtil

서브넷 계산할 때 사용하는 유틸리티입니다.

### SubnetMaskToBit

서브넷 마스크를 비트로 변환합니다.  예: 255.255.255.0 -> 24

### SubnetByteToString

바이트로 된 서브넷을 문자열로 변환합니다.

