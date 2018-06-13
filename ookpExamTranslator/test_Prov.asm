format PE GUI
entry start

struc class1 VarClass1, VarClass2, VarClass3, Met1
{
        .VarClass1 dd VarClass1
        .VarClass2 dd VarClass2
        .VarClass3 dd VarClass3
        .Met1:
                push ebp                        ;// пролог: сохранение EBP
                mov ebp, esp            	;// пролог: инициализация EBP
                mov eax,[ebp+8]  		;// доступ к аргументу
                mov [.VarClass3],eax
                pop ebp                		; // эпилог: восстановление EBP
                ret 12

}


section '.data' data readable writeable
        var1 dd 2
        cls class1 1,1,2

  _caption db 'Win32 assembly program',0
  _message db 'Hello World!',0



section '.code' code readable executable
        start:
                push 1
                call cls.Met1

                if var1 eq 1
                   goto w
                end if

        push    0
        push    _caption
        ;push    cls.VarClass3
        push    _message
        push    0
        call    [MessageBoxA]

        push    0
        call    [ExitProcess]

        w:
                mov [var1], 0
                mov ecx, [var1]
                if ecx eq 1
                   loop w
                end if


section '.idata' import data readable writeable

  dd 0,0,0,RVA kernel_name,RVA kernel_table
  dd 0,0,0,RVA user_name,RVA user_table
  dd 0,0,0,0,0

  kernel_table:
    ExitProcess dd RVA _ExitProcess
    dd 0
  user_table:
    MessageBoxA dd RVA _MessageBoxA
    dd 0

  kernel_name db 'KERNEL32.DLL',0
  user_name db 'USER32.DLL',0

  _ExitProcess dw 0
    db 'ExitProcess',0
  _MessageBoxA dw 0
    db 'MessageBoxA',0

section '.reloc' fixups data readable discardable       ; needed for Win32s


