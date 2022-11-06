# Orthoverse0

本リポジトリは、東京Web3ハッカソン（https://tokyo.akindo.io）の提出物となります。

## プロダクト概要

Orthoverse0は、NFTを用いたリアルメタバースプラットフォームのプロトタイプです。
本システムは、現実の場所とリアルメタバース上の情報・データをNFTにより結びつけ、空間アプリのための基盤となります。
まず、ERC721をベースとするコントラクトにより、空間IDをTokenIDとして、空間ID-URI文字列をブロックチェーン上で管理します。
コントラクトに定額の支払いをすることで、誰でもmintすることができ、このNFTを所有・取引できます。
空間IDは、地球上の特定の矩形領域（おおよそ20m四方）を表しています。そのため、NFTの所有者は任意のURIをその場所に紐付けることができます。
クライアントARアプリは、起動すると端末位置の緯度経度をGeospatialAPIにより取得し、周辺のNFTから各場所に紐付いているURIを取得します。
このURIの示すWebページには、独自のマークアップ言語により動的な3Dコンテンツを置くことができ、これをクライアントアプリは読み込んで現実にAR表示します。
これにより、現実空間に様々なサービスを展開することができます。

## 資料等について

以下提出資料として作成したスライドと動画へのリンクです。
https://docs.google.com/presentation/d/e/2PACX-1vRmd-El_hbhpAfPfamzjP4wsG16xjiHUZh04wxq44Q3hjI7jNhpsATvvlldHD6vaEXSAqjMabzI5HRY/pub?start=false&loop=false&delayms=3000
https://drive.google.com/file/d/1f3_OmrLSKHuyDMxhGu_qFRXwtMzB29bX/view?usp=sharing

テストネットワーク対象のmintサイトが以下です。
https://ortv.tech/map.html


## 使用したTech Stacks

### Contract

Solidity
Truffle
Ganache(for local dev)
openZeppelin(ERC721Enumerable)
dotenv

### Server

AWS
Let's Encrypt(For SSL Certs)

### External APIs

Infura(as Ethereum network endpoint)

### Web Interface

web3.js
Vite
OpenLayers

### Metadata

Python
Flask
mod_wsgi

### Map Image Server

OpenStreetMap(mapdata)
PostgreSQL with PostGIS
renderd & mapnik
Ubuntu 22.04
Apache2
mod_tile

### Client

Unity 2022.3.9f1
Nethereum
UniVRM/UniGLTF
ARFoundation
ARCore Extention (Google Geospatial API)
PLATEAU
Orthoverse HOML Core (https://github.com/oho-sugu/HOMLCore)

## 使用したBlockchain

Ethereum（Goerliテストネット）

## DeployしたContract

0x0D03EFbaccC2f53126bc832c66082ACaf5947B98
https://goerli.etherscan.io/address/0x0D03EFbaccC2f53126bc832c66082ACaf5947B98

## フォルダの内容について

orthoverse-contracts コントラクトのプロジェクト
orthoverse-metadata メタデータ用のJSONを動的生成するためのWebAPI
orthoverse-web NFTをmintするためのサイト
Unity クライアントUnityプロジェクト

## プロジェクトにアクセスする方法

### 1. Mint

まず、以下のmintページにアクセスします。
https://ortv.tech/map.html

[Enable Ethereum]ボタンを押すとWeb3.jsによりMetamaskなどにつながります。
（今回はMetamaskのみ動作確認しています。）

地図画面は、一般的なWeb地図と同様にドラッグ移動や拡大縮小ができます。
クリックすると、矩形が表示されます。
（拡大率によっては、右側に「More Zoom」というメッセージが表示されます。その場合はより拡大してください。）
すでに誰かによって確保されている領域の場合は、右側にURIが表示されます。
表示されていなければ、所有者のいない領域なので、テキストフィールドにURIを入力して[Mint]ボタンを押し、
ウォレットの指示にしたがってトランザクションを実行することでmintできます。

ここまでで、矩形領域に対して、自分のURIを設定することができました。

※コントラクトには所有者がURIを変更するためのAPIもありますが、今回はWebインターフェースでは使えません。Etherscanなどで実行します。

※作成したNFTはOpenSea（テストネット）での取引ができることは確認しました。メタデータの表示もOpenseaで可能となっています。

### 2. コンテンツ作成

マークアップで記述したテキストを任意の外部からアクセスできるWebサーバーにデプロイします。
マークアップの仕様は、Mozillaが開発したA-Frame(https://aframe.io)をベースにしています。
（Unity上でパーサーから作っているので、厳密には互換性はない、また、Lua言語を使ったスクリプティングなど独自拡張もしている。）
サンプルは次のURLにいくつか作成しています。https://github.com/oho-sugu/ovtestpages
Webページとしての公開は、GitHub Pagesなどが便利です。

### 3. クライアントアプリ

Unityフォルダ内のプロジェクトをビルドして、端末にインストールします。
自分でビルドする際には、Geospatial APIとInfuraのAPIキーを作成して、所定の箇所に設定してください。
InfuraのAPIキーは、`OrthoverseZero/Unity/Assets/Scripts/PlaceManager.cs`に設定箇所があります。
Geospatial APIのAPIキーは、Google Cloudのコンソールで作成し、`Project Settings`-`ARCore Extensions`で設定します。
現時点では、Androidでのビルドと動作確認のみできています。

屋外でアプリを起動して、URIをNFTで紐付けた場所に行くと、2で作成・公開したコンテンツをAR表示できます。

### 4. さらに先に行くには

現状の実装ではまだできることが限られますが、基本的なWebアプリの作成の知識があれば、簡単な空間アプリの作成はできると思います。
アクセスURLからダイナミックにコンテンツを生成するWebサービスを紐付けることで、空間アプリを作ることができます。
また、GLTF/GLB/VRM/OBJなどの形式の3Dモデルを読み込むこともできるので、それらを活用できるでしょう。

## 権利等について

本プロジェクトの権利については、作者実装分については作者に帰属します。
個人の興味範囲での使用以外の利用（商用利用等）の場合はお問い合わせください。

## 詳解Orthoverse0

本プロダクトは、将来ARグラスが普及した未来を想定しています。
ターゲットは、廉価・高性能・軽量なHoloLens2のようなデバイスとイメージしていただければと思います。

マークアップ言語は、HTMLとJavaScriptに似た仕組みとなっており、簡単な動的コンテンツやサーバーと連携したコンテンツも作成できます。
Webサーバーとの組み合わせにより、現在のWebでできていることを空間アプリとして現実世界で実現できます。

・・・

TBD

