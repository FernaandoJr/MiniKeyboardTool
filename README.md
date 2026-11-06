# MINI KeyBoard Tool — reconstrução fiel e editável

Reconstrução do código-fonte do **MINI KeyBoard.exe** (produto interno `HIDTester` v1.0.0.0),
o software de configuração dos mini teclados macro USB HID genéricos (3 teclas × 3 camadas,
LED, mouse, multimídia), com base no release de fábrica de 2022.

## Objetivo

Mexer na **interface** (WinForms) sem quebrar o resto. A camada HID defensiva
(`HID/*`, P/Invokes em `hid.dll`/`setupapi.dll`/`kernel32`/`user32`), o payload de gravação
(botão *Download*, que escreve pacotes direto no handle HID) e todos os recursos de UI
estão preservados byte-por-byte em relação ao original.

## Estrutura

```
src/HIDTester/          # Fonte C# (.NET Framework 4.0, WinForms) + todos os .resx da UI
    HID/                # P/Invokes hid/setupapi/kernel32/user32 (NÃO MEXER — é o protocolo)
    HIDTester/          # FormMain + telas (BasicKeys, FunKey, LayerFun, LEDkey, MouseKey, MULKey)
    MINI KeyBoard.csproj  # csproj clássico, compila com o MSBuild 4.0 do próprio Windows
reference/              # Binários originais de fábrica (SHA-256 4ae53d3a...cd7b1)
tools/ilspycmd/         # Decompilador self-contained (NuGet ilspycmd.win-x64 11.2.0.10050)
verify/                 # Scripts de verificação
```

Os arquivos de interface que você mexe são:
`src/HIDTester/HIDTester/*.cs` (código) e `src\HIDTester\*.resx` (layout, textos, imagens
do designer — a UI inteira).

## Como compilar (sem instalar nada)

```
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe ^
  "src\HIDTester\MINI KeyBoard.csproj" /p:Configuration=Release /t:Rebuild
```

Saída: `src\HIDTester\bin\Release\MINI KeyBoard.exe` (protoc x86, .NET 4.0, título original
"MINI KeyBoard V02.1.1"确认).

## Validade da reconstrução (evidência)

- **30 tipos / 295 métodos / 331 campos** idênticos ao original (REA `compare_managed_members`:
  350 members *unchanged*; os demais mudam só por tokens de metadata local ao build).
- **Resources embutidos**: 8/8 presentes; **7/8 byte-idênticos** por SHA-256
  (`FunKey`, `LayerFun`, `LEDkey`, `MouseKey`, `MULKey`, `Properties.Resources`, `BasicKeys`).
- `HIDTester.FormMain.resources`: conteúdo **byte-idêntico até o fim real** (34.489 bytes;
  304 chaves exatamente iguais). O original carrega 841 bytes **finais não-referenciados**
  (registros duplicados escritos pelo compilador defeituoso do fabricante — mesmo defeito
  das 3 user strings com flag fora do padrão ECMA-335 apontadas em análise estática).
  O runtime ignora esses bytes; nada a preservar.
- Smoke test: o rebuild abre com o window title original `MINI KeyBoard V02.1.1`.

## Limitações conhecidas

- Reconstrução é estática; recursos de serialização (fontes/ícones) não foram
  desserializados em memória durante a comparação (limitação igual nos dois lados; as
  entradas batem por chave e por bytes).
- A gravação real no teclado (botão Download) só foi testada com o original; o rebuild
  usa as mesmas chamadas HID (verificado por comparação de calls/P-Invoke estáticos).

## Como verificar de novo

```
pwsh -NoProfile -File verify\bytediff.ps1     # compara orig vs rebuild .resources
```
(ajuste os paths dentro do script se trocar os binários)

