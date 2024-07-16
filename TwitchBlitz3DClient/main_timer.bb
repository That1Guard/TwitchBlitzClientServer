Include "timer.bb"

Graphics3D 800,600,32,1

; This code is setup to contain test example's to use for testing the timer.
Type ExampleType
    Field name$
    Field value
End Type


Function YourFunctionName1(paramsList$)
    Print "Function 1 executed in main with parameters: " + paramsList$
End Function

Function YourFunctionName2(paramsList$)
    Local example.ExampleType = New ExampleType
    example\name = paramsList$
    example\value = Rand(1, 100)
    Print "Function 2 executed in main with ExampleType: Name=" + example\name + ", Value=" + example\value
End Function

; Now, we begin the timer test.
; Create a timer that calls YourFunctionName1 every 1000 milliseconds.
Local timer1 = CreateTimer("YourFunctionNameHere1", 1000, True, "Hello")
Print "Timer 1 created: " + timer1

; Create a timer that calls YourFunctionName2 every 9000 milliseconds.
Local timer2 = CreateTimer("YourFunctionNameHere2", 9000, False, "Cake")
Print "Timer 2 created: " + timer2

; Create a timer that calls ssefghdrh every 5000 milliseconds and throw not found function.
Local timer3 = CreateTimer("ssefghdrh", 5000, False, "Tasty")
Print "Timer 3 created: " + timer3

; Main loop what will run the timers for 10 seconds of program execution.
Local startTime = MilliSecs()
While MilliSecs() - startTime < 10000
    UpdateTimers()
	;Print("Time currently Millecs: " + (MilliSecs() - startTime))
    ;Delay 5  ; Adjust the delay as needed when used
Wend

; Testing for removing a specific timer.
Print "[PAUSED] Press any key to continue with testing: Remove Timer 1"
WaitKey()
Print "Removed timer'" + timer1 + "' with status: " + RemoveTimerObject(timer1)

; Testing for removing all the remaining timers.
Print "[PAUSED] Press any key to continue with testing: Remove All Remaining Timers"
WaitKey()
Print "Cleaning and removing Timers from array"
For t.Timer = Each Timer
	RemoveTimer(t)
Next

; Finally, we exit the testing application.
Print "[PAUSED] Press any key to now exit test"
WaitKey()

;~IDEal Editor Parameters:
;~C#Blitz3D - USE THIS ONLY