# OcuBri
`OcuBri`はwebvr-polyfillに追加したWebSocketBridgeHMDPositionSensorVRDevice用のWebSocketブリッジアプリです。
Oculus Riftの拡張モード(Extend Desktop to the HMD)のみ対応しています。

##特徴
1. Webから接続されるとOculus Riftのユーザープロファイルを取得し、ブラウザーに送信します。
これにより、ユーザーに合わせた表示ができるのではと考えています。(いかんせん私の3Dに関する知識がまったくと言っていいほどないためこのデータを生かすことができていませんが)
2. WebVR APIと同様の使い勝手を目指して、ブラウザーからリクエストがくると、Oculus Riftのモニターの位置を取得し自動的にOculus Riftに表示されます。(ちょっと見てくれが悪いですが) 
