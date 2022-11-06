var PNS = artifacts.require("PNSRegistry");
module.exports = function(deployer) {
  deployer.deploy(PNS);
};