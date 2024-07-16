Local result = ConnectToServer("127.0.0.1", 11000)

; Kinda works, not really. Checks for connection issues.
If result = 0
    Print "Connected to server"
Else
    Print "Failed to connect, error code: " + result
End If

; We begin the server by sending the start command to it.
SendDataToServer("!vstart")

; Begin main loop.
While Not KeyHit(1)
	If IsNewDataAvailable() Then
		Local response$ = ReceiveDataFromServer()
		If response$ <> "recvfrom failed" And response$ <> "Connection closed" Then
			Print "Received: " + response$
			; Parses and breaks down the command before we take appropriate action.
			Local commandEnd = Instr(response$, " ")
			If commandEnd > 0 Then
				Local command$ = Left(response$, commandEnd - 1)
				Local parameter$ = Mid(response$, commandEnd + 1)
				
				; This is where you pass off the command types where it will be handled somewhere else.
				Select command$
					Case "command:"
						Print "Handle console commands with parameter:'" + parameter$ + "'"
					Case "particle:"
						Print "Handle particle data with parameter:'" + parameter$ + "'"
					Default
						Print "Unknown command: " + command$
				End Select
			Else
				Print "Invalid response format"
			End If
			
			; Deal or no deal; Continue the loop or stop.
			Local inputKeyWait = WaitKey() 
			If inputKeyWait <> 27
				SendDataToServer("!vstart")
			End If
		End If
	End If
    Delay 100
Wend

; Cleanup
DisconnectFromServer()

;~IDEal Editor Parameters:
;~C#Blitz3D - USE THIS ONLY