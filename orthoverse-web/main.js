import './style.css';
import {Feature, Map, View} from 'ol';
import TileLayer from 'ol/layer/Tile';
import VectorLayer from 'ol/layer/Vector';
import VectorSource from 'ol/source/Vector';
import OSM from 'ol/source/OSM';
import XYZ from 'ol/source/XYZ';
import { transform, toLonLat, fromLonLat } from 'ol/proj';
import { Geometry, LineString } from 'ol/geom';
import Web3 from 'web3';
import PNSabi from './PNSRegistry.json';

if (typeof window.ethereum !== 'undefined'){
  console.log('Metamask is installed');
}

const ethereumButton = document.querySelector('.enableEthereumButton');
const showAccount = document.querySelector('.showAccount');

ethereumButton.addEventListener('click', () => {
  getAccount();
});

var account;
async function getAccount() {
  const accounts = await ethereum.request({ method: 'eth_requestAccounts' });
  account = accounts[0];
  showAccount.innerHTML = account;
}

const web3 = new Web3(Web3.givenProvider);
var PNSRegistryContract = new web3.eth.Contract(PNSabi.abi,'0x0D03EFbaccC2f53126bc832c66082ACaf5947B98');

const source = new VectorSource({wrapX: false});
const vector = new VectorLayer({source: source,});

const map = new Map({
  target: 'map',
  layers: [
    new TileLayer({
      source: new XYZ({
        attributions: [
        OSM.ATTRIBUTION,
          'Tiles courtesy of ' +
          '<a href="http://openstreetmap.org">' +
          'OpenStreetMap' +
          '</a>'
        ],
        url: 'https://ortv.tech/ortv/{z}/{x}/{y}.png'
      })
    }),
    vector
  ],
  view: new View({
    center: fromLonLat([139.72939,35.73071]),
    zoom: 18
  })
});

map.on('click', function (evt) {
  var message = document.getElementById('message');
  const lonlat = toLonLat(evt.coordinate);
  if((map.getView().getZoom()>=18)){
    message.textContent = lonlat;
    const tileinfo = deg2tile(lonlat[0], lonlat[1], 20,);
    const spatialcode = tileinfo.x.toString().padStart(7,'0')+tileinfo.y.toString().padStart(7,'0');

    source.clear();

    const coords = [
      tile2deg(tileinfo.x,tileinfo.y,tileinfo.z),
      tile2deg(tileinfo.x+1,tileinfo.y,tileinfo.z),
      tile2deg(tileinfo.x+1,tileinfo.y+1,tileinfo.z),
      tile2deg(tileinfo.x,tileinfo.y+1,tileinfo.z),
      tile2deg(tileinfo.x,tileinfo.y,tileinfo.z)
    ]
    var lines = new LineString(coords);
    lines.transform('EPSG:4326', 'EPSG:3857');
    const box = new Feature({geometry: lines});

    source.addFeature(box);

    PNSRegistryContract.methods.getRecord(spatialcode).call(function(err,res){
      console.log(res);
      message.innerHTML = lonlat+"<br>"+spatialcode+"<br>"+res+"<br><input type='url' id='url' required><br><button class='mintButton'>Mint</button>";
      const mintButton = document.querySelector('.mintButton');
      mintButton.addEventListener('click', () => {
        console.log(spatialcode);
        var url = document.getElementById('url').value;
        PNSRegistryContract.methods.mint(account,spatialcode,url).send({
          from: account,
          value: web3.utils.toWei('0.01','ether'),
        }).on('transactionHash', function(hash){
          console.log(hash);
        }).on('receipt', function(receipt){
          console.log(receipt);
        }).on('confirmation', function(confirmationNumber,receipt){
          console.log(confirmationNumber);
          console.log(receipt);
        }).on('error',console.log);
      });
    });

    console.log(lonlat);
    console.log(spatialcode);

  } else {
    message.innerHTML = lonlat+"<br>ZL="+map.getView().getZoom()+"<br>More Zoom"
  }
});

const deg2tile = (lon, lat, zoom, options) => {
  const { min, max } = Object.assign({}, options);
  let z = min && min > zoom ? min : max && max < zoom ? max : zoom;
  z = z > 0 ? Math.ceil(z) : 0;
  return {
    x: Math.floor(((lon + 180) / 360) * Math.pow(2, z)),
    y: Math.floor(
      ((1 - Math.log(Math.tan((lat * Math.PI) / 180) + 1 / Math.cos((lat * Math.PI) / 180)) / Math.PI) / 2) * Math.pow(2, z)),
    z,
  };
};

const tile2deg = (tilex,tiley,zoom) => {
  var n=Math.PI-2*Math.PI*tiley/Math.pow(2,zoom);
  return [
    (tilex/Math.pow(2,zoom)*360-180),
    (180/Math.PI*Math.atan(0.5*(Math.exp(n)-Math.exp(-n))))
  ];
}