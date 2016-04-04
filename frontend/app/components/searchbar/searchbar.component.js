(function(){
  'use strict';

  angular
    .module("CRM")
    .component("searchBar", {
      templateUrl: "view/app/components/searchbar/searchbar.html",
      controller: Controller,
      controllerAs: "vm",
      bindings: {
        searchData: "="
      }
    });

  function Controller(){
    var vm = this;
    vm.term = "";
    vm.results = [];

    vm.search = search;

    function search(){
      var results = [];
      for(var searchElem of vm.searchData){
        console.log(searchElem);
        results.push(searchElem.searchFun(vm.term));
      }
      vm.results = _.flatten(results);
    }
  }
})();
