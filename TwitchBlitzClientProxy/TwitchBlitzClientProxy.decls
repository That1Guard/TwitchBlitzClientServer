.lib "TwitchBlitzClientProxy.dll"

ConnectToServer%( ipAddress$, port% ):"_ConnectToServer@8"
SendDataToServer%( data$ ):"_SendDataToServer@4"
IsNewDataAvailable%():"_IsNewDataAvailable@0"
ReceiveDataFromServer$():"_ReceiveDataFromServer@0"
DisconnectFromServer():"_DisconnectFromServer@0"