# AutoReplenishItem

## What's this?

Automatically replenishes items when the storage is opened.  
You need to set the number of items for each item in advance.  

I've been working on a conversation in the #mod作成 channel of the Dyson Sphere Program_Jp server on Discord.  
If you have any questions, please feel free to contact me there.  

## CONFIG

When you first run the DSP after installing a mod, a list of item names, item IDs, and quantity will be output to the configuration file.  
The number of items is initially set to 0, so you can set the number of items you want to replenish automatically.  
The format is item name/item ID/count of items, but the mods will ignore whatever you write in the item name.  
If you want to reset the list, set the Reset value to true and load it. If you want to reset the list, set the Reset value to true and load it, but be aware that all settings will be lost.  

Reset = false  
ItemNameIdCountNNN = item name/item id/count  

## CHANGE LOG

### v0.0.1

 - Publish.

## これは何？

ストレージを開けたときに自動でアイテムを補充します。  
事前にアイテムごとに個数を設定する必要があります。  

DiscordのDyson Sphere Program_Jpサーバの#mod作成チャンネルで会話をしながら作っていたので、  
何かあればそこで私に声をかけて頂ければと思います。  

## 設定

MODを入れたあとDSPを最初に起動したときにアイテム名、アイテムID、個数の一覧が設定ファイルに出力されます。  
個数は最初は全て０に設定されているので、自動で補充したいアイテムの個数を設定してください。  
形式は アイテム名/アイテムID/個数 となっていますが、アイテム名のところはMODは何を書いてもMODは無視します。  
一覧をリセットしたい時はResetの値をtrueにしてロードしてください。そのとき設定済みのものは全て消えるので気をつけてください。  

Reset = false  
ItemNameIdCountNNN = item name/item id/count  

## 変更履歴

### v0.0.1

 - 公開
