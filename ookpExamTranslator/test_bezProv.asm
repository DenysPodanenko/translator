format PE GUI
entry start

struc class1 
{
        .a dd ?
        .b dd ?
        .c dd ?
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

section '.code' code readable executable
        start:
                push 1
                call cls.Met1

                if var1 eq 1
                   goto w
                end if
        w:
                mov [var1], 0
                mov ecx, [var1]

                if ecx eq 1
                   loop w
                end if