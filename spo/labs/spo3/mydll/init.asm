; Процедура инициализации для DLL
.386
.model flat, stdcall
option casemap: none

.code
DllMain proc hInstance:dword, reason:dword, lpReserved:dword
	mov eax, -1
	ret
DllMain endp
end DllMain