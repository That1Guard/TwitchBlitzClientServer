Type Timer
    Field interval
    Field updater
    Field loop
    Field timerfunc$
    Field paramsList$
End Type

; -------------- Timers ----------------

Function CreateTimer(public$, interval, loop, databank$)
    Local t.Timer = New Timer
    t\interval = interval
    t\updater = MilliSecs() + interval
    t\loop = loop
    t\timerfunc = public$
    t\paramsList = databank$
	
    Return Handle(t)
End Function

Function UpdateTimers()
    For t.Timer = Each Timer
		If t\updater < MilliSecs() Then
            Local paramsList$ = t\paramsList
            Select FunctionNameFromTimer(t)
                Case "YourFunctionNameHere1"
                    YourFunctionName1(paramsList)
                Case "YourFunctionNameHere2"
                    YourFunctionName2(paramsList)
				Default
					NoFunctionFound()
			End Select
			
			If t\loop Then
				t\updater = MilliSecs() + t\interval
			Else
				RemoveTimer(t)
			End If
			
		End If
	Next
End Function

Function RemoveTimer(t.Timer)
    Delete t
End Function

Function RemoveTimerObject(timer)
	RemoveTimer(Object.Timer(timer))
End Function
	
; ------------- Test Functions ----------------

Function FunctionNameFromTimer$(t.Timer)
    Return t\timerfunc$
End Function

Function NoFunctionFound()
	Print "No Function found during execution."
End Function

;~IDEal Editor Parameters:
;~C#Blitz3D - USE THIS ONLY