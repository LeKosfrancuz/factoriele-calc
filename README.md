# Kalkulator za faktorijele
Ovo je potpuno funkcionalan kalkulator (prioritet operacija, zagrade, množenje, djeljenje, potenciranje, itd.) koji može računati i faktorijele.

## Quick Start
Za Windows i Linux:
```console
$ git clone https://github.com/LeKosfrancuz/factoriele-calc.git .
$ cd Kalkulator/
$ dotnet run
```
Za VisualStudio samo treba pokrenuti Kalkulator.sln i iz grafičkog sučelja kompajlirat projekt.

## Komande
od verzije 2.5 sljedece komande su dostupne:

### DEF komanda
Korištenje: `DEF [ImeVarijable]` <br>
Kreira varijablu s imenom [ImeVarijable] i zatraži upis vrijednosti.
<br>
> [!IMPORTANT]
> Kreiranje nove varijable moguće je i s dodjeljivanjem vrijednosti nedefiniranoj varijabli

### HELP komanda
Prikazuje help meni s opisom dostupnih komandi.
> ***NOTE: `?` se također može koristiti umjesto `HELP`***

### Komanda za gašenje programa
Upisivanjem `ESC` program se gasi.
> ***NOTE: `Q` se također može koristiti umjesto `ESC`***

### Komanda za čiščenje ekrana
Upisivanjem `CLS` ekran kalkulatora potpuno će se obrisati

### Komanda za ispis statističkih podataka i vremena
Korištenje: `TIME [matematicki izraz za evaluirati]` <br>
Mjeri vrijeme izvodenja funkcija faktorijela matematickog izraza i omogucuje dodatnu statistiku za operaciju izračunavanja faktorijela Stirlingovom metodom. <br>
Radi preciznosti mjerenja, svako mjerenje izvodi se 100 000 puta, pa preporučamo manje brojeve.

```console
> TIME 2!s
Apsolutna greska: 0.08099564851101682
Relativna greska: 0.04049782425550841

Vrijeme: 0.036 µs
1.9190043514889832
```

## Operacije
Operacije `A + B`, `A - B`, `A * B`, `A / B`, `A = B` <br>
- Moraju imati 2 ili vise operanda npr (A = B, A + B, ...) <br>
- Operandi su imena varijabli i brojevi (numericke vrjednosti)

### Potenciranje
Operacije potenciranja: `A^B` <br>
- Moraju imati 2 operanda <br>
- A - broj ili varijabla <br>
- B - broj ili varijabla <br>

### Faktorijele
Operacija faktorijel: `A![mode]` <br>
- mode default: `o` <br>
- Modovi: <br>
`r` - evaluacija rekurzijom <br>
`o` - optimizirana evaluacija (trenutno koristi for petlju) <br>
`b` - optimizirana evaluacija za jako velike brojeve <br>
`s` - evaluacija Stirlingovom aproksimacijom <br>

#### Primjer korištenja
Kao normalni kalkulator:
```console
> 12!
479001600
```
Korištenje u kombinaciji s funkcijom mjerenja:
```console
> TIME 12!r
Vrijeme: 0.138 µs
479001600
> TIME 12!
Vrijeme: 0.032 µs
479001600
```
Pokaz relativne pogreške u kalkulatoru
```console
> (12! - 12!s) / 12!
0.006918794273806068
```

> [!IMPORTANT]
> Funkcije faktorijela bazirane na double-u (sve osim `b`) nakon broja 170 evaluiraju se u beskonačnost, u edukativne svrhe svi brojevi preko 170 automatski će biti izračunati u `b` modu i ispisani na ekran
> (u jednađbi se i dalje gleda kao beskonaćnost)
