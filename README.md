# OcuBri
`OcuBri`はフォークした[webvr-polyfill](https://github.com/gtk2k/webvr-polyfill)に追加したWebSocketBridgeHMDPositionSensorVRDevice用に作成したWebSocketブリッジアプリです。

##対応状況
Oculus Riftの拡張モード(Extend Desktop to the HMD)のみ対応しています。    
また、Win32Apiを使用しているため対応OSはWindowsのみとなります。  
ブラウザーは安定版のみ対応しています。(OcuBri内部でWindowsのレジストリからexeファイルのパスを取得しているため)

##特徴
1. ブラウザーから接続されるとOculus Riftのユーザープロファイルを取得し、ブラウザーに送信します。
これにより、ユーザーに合わせた表示ができるのではと考えています。(いかんせん私の3Dに関する知識がまったくと言っていいほどないためこのデータを生かすことができていませんが)
2. WebVR APIと同様の使い勝手を目指して、ブラウザーからリクエストがくると、Oculus Riftのモニターの位置を取得し自動的にOculus Riftに表示されます。(ちょっと見てくれが悪いですが) 

##問題点
* IE11やOpera、Vivaldiでも動作するように目指していたのですが、IE11ではWebGL自体はサポートしているのですがビデオテクスチャーがサポートされていません。ビデオテクスチャーを使用していないWebGLコンテンツなら大丈夫だと思いますが。
* OperaやVivaldiはOculus用ウィンドウを生成してOculus Riftに移動させるというのが難しものとなっているため、この２つのブラウザーは現時点ではサポートから外しています。

##動作確認用サンプルページ
[こちら](https://github.com/gtk2k/gtk2k.github.io)に動作確認用サンプルページを用意しました。
