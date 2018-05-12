format PE GUI
entry start
struc class1;class class1
{
.ab dd ?
.b dd ?
.c dd ?
.Met1:;method Met1
push ebp
mov ebp, esp
mov eax,[ebp+8]
mov [.c], eax
f dd 2;f=2
mov [.c], f
pop ebp
ret 12
}

section '.data' data readable writeable 
var1 dd 2;var1 = 2
cls class1;new class cls
var3 dd 2;

section '.code' code readable executable
start:
push 2
call cls.Met1
if var1 eq 1
goto w1
end if
w1:
mov [var1], 0
mov ecx, [var1]
if ecx eq 1
loop w1
end if
