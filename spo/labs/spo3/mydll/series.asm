.686
.model flat, stdcall
option casemap: none

include \masm32\include\windows.inc
include \masm32\include\kernel32.inc
include \masm32\include\masm32.inc
include \masm32\include\msvcrt.inc
includelib \masm32\lib\kernel32.lib
includelib \masm32\lib\masm32.lib
includelib \masm32\lib\msvcrt.lib

TABLE_WIDTH  equ 20
COLUMN_COUNT equ 4
BUFFER_SIZE  equ TABLE_WIDTH

print macro args:REQ
	irp arg, <args>
		invoke WriteConsole, handleOut, addr arg, length arg, NULL, NULL
	endm
endm

printS macro arg:REQ
	invoke WriteConsole, handleOut, addr arg, sizeof arg, NULL, NULL
endm

floatToString macro arg:REQ
	call clearBuffer
	invoke FloatToStr, arg, offset buffer
endm

input macro arg:REQ
	invoke crt_scanf, addr format, addr arg
endm

padLeft macro arg:REQ
	invoke WriteConsole, handleOut, addr arg, sizeof arg, NULL, NULL
	rept TABLE_WIDTH - sizeof arg
		print space
	endm
endm

.data
	; ���������
	message1 db 'xStart: ', 0
	message2 db 'xEnd: ', 0
	message3 db 'deltaX: ', 0
	message4 db 'Precision: ', 0
	message5 db 'Alpha: ', 0
	
	; ��������� �������
	header1 db 'Argument', 0
	header2 db 'Series sum', 0
	header3 db 'a^x', 0
	header4 db 'Element count', 0
	
	; �������� ��������
	currentX  dq ?
	finalX    dq ?
	deltaX 	  dq ?
	precision dq ?
	alpha	  dq ?
	
	; ������ �������
	function  dq ?
	seriesSum dq ?
	element   dq ?
	elemCount dq 0
	
	; ����-�����
	handleIn  dd ?
	handleOut dd ?
	buffer	  db BUFFER_SIZE dup(0)
	format 	  db "%lf", 0
	
	; �������������
	upperLeft      db 201, 0
	upperRight     db 187, 0
	upperT         db 203, 0
	lowerLeft      db 200, 0
	lowerRight 	   db 188, 0
	lowerT         db 202, 0
	leftT          db 204, 0
	rightT         db 185, 0
	cross      	   db 206, 0
	verticalLine   db 186, 0
	horizontalLine db TABLE_WIDTH dup(205), 0	
	space		   db ' ', 0
	newline		   db 10, 13, 0
	
.code
; ��������� ����� ���� � ���-�� ���������
getSeriesSum proc uses eax
	local count: dword
	local lnax: qword
	
	mov count, 0
	finit
	
	fld currentX	; x --> st(1)
	fld alpha		; a --> st(0)
	fyl2x 			; log2a*x
	fldln2			; \ ln2*log2a*x = lna*x
	fmul			; /
	fstp lnax		; ��������� �������� lna*x
	
	fldz				; ����� = 0
	fld1				; \ ������� = 1
	fstp element		; /
	
@loop:  fld element 
		fadd
		inc count
		
		fld element		; \ 
		fld lnax		;  | �������� ������� �� lna*x
		fmul			; /
		
		cmp count, 2	; ��������� ����� ������ ����� 2 ��������
		jl @f
		fidiv count		; ����� �� n
		
@@:		fstp element
		push offset precision
		push offset element
		call compare
	ja @loop
	
	fstp seriesSum
	fild count
	fstp elemCount
	
	ret
getSeriesSum endp

; ��������� �������� ������� a^x
calcFunction proc
	local trunc:dword
	
	finit
	fld currentX	; x --> st(1)
	fld alpha		; a --> st(0)
	
	; a^x = 2^(log2a*x)
	fyl2x 		; log2a*x
	fist trunc	; ��������� ����� ����� ���������	
	fild trunc	; ����� � � ���� FPU
	fsub		; �������� �� ���������, �������� ������� �����
	f2xm1 		; 2^decimalPart(log2a*x) - 1
	fld1		; \ result++
	fadd		; / 
	fild trunc	; ����� ����� ����� � FPU
    fxch		; swap(st0, st1)
    fscale		; result * 2^trunc(log2a*x) = 2^(log2a*x) = a^x
	
	fstp function		; result --> function
	
	ret
calcFunction endp

; ���������� ��� ����� double
compare proc uses eax X:dword, Y:dword 
	mov eax, Y
	fld qword ptr [eax]
	mov eax, X
	fld qword ptr [eax]
	fcompp	 	; ��������� ���� ����� � �������� �� �� �����
	fstsw ax 	; ��������� ��������� ����� � ��
	sahf		; ��������� ���������� �� � ������� ������
	ret 8
compare endp

; ������� �����
clearBuffer proc uses eax cx edi
	mov ecx, BUFFER_SIZE / 4
	lea edi, buffer
	xor eax, eax
	cld
@@: stosd
	loop @b
	ret
clearBuffer endp

drawHeader proc
	print upperLeft
	rept COLUMN_COUNT - 1
		print horizontalLine
		print upperT
	endm
	print <horizontalLine, upperRight, newline>
	
	irp header, <header1, header2, header3, header4>
		print verticalLine
		padLeft header
	endm
	print <verticalLine, newline>
	
	print leftT
	rept COLUMN_COUNT - 1
		print horizontalLine
		print cross
	endm
	print <horizontalLine, rightT, newline>
	
	ret
drawHeader endp

drawRow	proc
	irp value, <currentX, seriesSum, function, elemCount>
		print verticalLine
		floatToString value
		padLeft buffer
	endm
	print <verticalLine, newline>
	ret
drawRow endp

drawTable proc
	call drawHeader
	
@@: call getSeriesSum
	call calcFunction
	call drawRow
	fld currentX
	fld deltaX
	fadd
	fstp currentX
	push offset finalX
	push offset currentX
	call compare
	jna @b

	print lowerLeft
	rept COLUMN_COUNT - 1
		print horizontalLine
		print lowerT
	endm
	print <horizontalLine, lowerRight>
	ret
drawTable endp
	
DllSeries proc uses eax 
	invoke GetStdHandle, STD_INPUT_HANDLE
	mov handleIn, eax
	invoke GetStdHandle, STD_OUTPUT_HANDLE
	mov handleOut, eax
	
	printS message1
	input currentX
	printS message2
	input finalX
	printS message3
	input deltaX
	printS message4
	input precision
	printS message5
	input alpha
	
	print newline
	call drawTable
			
	ret
DllSeries endp
end