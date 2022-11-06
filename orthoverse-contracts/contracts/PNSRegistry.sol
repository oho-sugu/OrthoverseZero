// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2022- Suguru Oho <oho.sugu@gmail.com>

pragma solidity >=0.4.22 <0.9.0;

import "@openzeppelin/contracts/utils/math/SafeMath.sol";
import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Enumerable.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

contract PNSRegistry is ERC721Enumerable, Ownable {
  using SafeMath for uint256;
  uint256 initialPrice = 0.01 ether;

  mapping(uint256 => string) private records;

  constructor() ERC721("OrthoverseZero", "ORTZ") {
  }

  function mint(address to, uint256 _spatialCode, string memory _url) public payable virtual {
    require(msg.value == initialPrice, "Not enough ehter");
    // 22bit 
    require(_spatialCode <= 0xFFFFFFFFFFF, "Spatial Code Range exceed");

    records[_spatialCode] = _url;
    _mint(to, _spatialCode);
  }

  function getRecord(uint256 _spatialCode) public view returns(string memory) {
    return records[_spatialCode];
  }

  function changeUrl(uint256 _spatialCode, string memory _url) public virtual {
    require(_exists(_spatialCode));
    require(ownerOf(_spatialCode) == msg.sender);

    records[_spatialCode] = _url;
  }

  function setInitialPrice(uint256 _initialPrice) external onlyOwner {
    initialPrice = _initialPrice;
  }

  function getInitialPrice() external view returns(uint256){
    return initialPrice;
  }

  function withdraw() external onlyOwner {
    require(address(this).balance > 0, "Not enough ether");
    address payable _owner = payable(owner());
    _owner.transfer(address(this).balance);
  }

  function _baseURI() internal view virtual override returns (string memory) {
    return "https://ortv.tech/meta/";
  }
}
