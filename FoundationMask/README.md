# FoundationMask

Mask the cell when put the Foundation.  
Can saving the Foundation.  

## CONFIG

\# Pattern X size.  
Mask.PatternSizeX = 3  
\# Pattern Y size.  
Mask.PatternSizeY = 3  

\# 1: Can put the Foundation.  
\# 0: Can not put the Foundation.  
Mask.Pattern0=100  
Mask.Pattern1=000  
Mask.Pattern2=000  

## CHANGE LOG

### v0.0.3

 - Add pattern size setting.

### v0.0.2

 - Add config.

## これは何？

環境改造で土台を置くとき、特定のセルのみに土台を置けるようにします。  
整地をするときに土台の消費を抑えることを意図して作成しました。  
設定ファイルで土台をどのように置くかを指定できます。  
設定ではパターンを指定し、そのパターンを星全体に繰り返して適用します。  
パターンの縦と横のサイズ、どこに土台を置けるか置けないかを設定します。  

## 設定

\# パターンの横幅  
Mask.PatternSizeX = 3  
\# パターンの縦幅  
Mask.PatternSizeY = 3  

\# 1: 土台を置けます  
\# 0: 土台を置けません  
Mask.Pattern0=100  
Mask.Pattern1=000  
Mask.Pattern2=000  

上記のように指定すると、3x3のうち1箇所だけ置けて以外は置けないパターンを星全体に繰り返します。  
そのように設定することで土台の消費を全て埋める場合と比べて9分の1に抑えられます。  
設定を書き換えた場合、ロードをすれば再取得できます。  
足りない設定や間違ってる設定は設定読込時に補完されます。  
不要な設定はDSPを起動するときに消されます。  

## 変更履歴

### v0.0.3

 - パターンサイズの設定項目追加

### v0.0.2

 - 設定ファイルを追加
