(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('App', App);

  App.$inject = ['dataservice'];
  function App(dataservice){
  //  var vm = this;

    activate();

    function activate() {
      dataservice.configToken();
    }

  }
})();
