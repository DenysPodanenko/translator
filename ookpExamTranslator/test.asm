format PE GUI
entry start

section '.data' data readable writeable
        var1 db 2
        cls Class1 <0,0,0>

section '.code' code readable executable
Class1 struct
        VarClass1 db ?
        VarClass2 db ?
        VarClass3 db ?
        Met1 proc
                push ebp           	// пролог: сохранение EBP
		mov ebp, esp       	// пролог: инициализация EBP
		mov VarClass3, [ebp+8]	// доступ к аргументу
		pop ebp            	// эпилог: восстановление EBP
                ret 12
        Met1 endp
Class1 ends
        start:
                push 2
                call cls.Met1
                ret
                .IF(var1 >=1) goto w
        w:
                mov var1, 0
                mov cx, var1
                loop w
        end start