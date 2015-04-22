# OcuBri
WebVRに対応していないブラウザー(注：問題点の項目をお読みください)でWebSocketを使ってOculus Riftを使用できるようにするブリッジアプリです。
[oculus-bridge](https://github.com/Instrument/oculus-bridge)とほぼ同じです。
ただし、OcuBriはwebvr-polyfillに今回実装したWebSocketBridgeHMDPositionSensorVRDevice用に作成したものですので、一般的に使いやすいものとはいいがたいものとなっています。

##特徴
1. Webから接続されるとOculus Riftのユーザープロファイルを取得し、ブラウザーに送信します。
これにより、ユーザーに合わせた表示ができるのではと思います。
2. Oculus Riftのモニターの位置を取得し、ブラウザーからリクエストがくると自動的にOculus Riftに表示されます。 

##ver0.0.1の仕様
一応、現時点の仕様を公開します。

使用ライブラリ
OvrCapi.cs(OculusPlugin.dll)とwebsocket-sharpを使用しています。(両者ともにごく一部分修正をして使用しています。)

