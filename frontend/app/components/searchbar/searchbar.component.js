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

  Controller.$inject = ['$q'];
  function Controller($q){
    var vm = this;
    vm.term = "";

    vm.search = search;

    activate();

    function activate(){
    }

    function search(){
      if(!vm.term){
        vm.results = undefined;
        return;
      }
      var tasks = [];
      for(var i in vm.searchData){
        tasks.push(vm.searchData[i]
                   .searchFun(vm.term, 3)
                   .then(function(result){
                     return result.data;
                   }));
      }
      $q.all(tasks).then(function(ts){
        for(var i in vm.searchData){
          if(ts[i].length < 1) continue;
          if(!vm.results) vm.results = {};

          vm.results[vm.searchData[i].key] = {
            label: vm.searchData[i].label,
            data: ts[i],
            link: vm.searchData[i].link,
            selector: vm.searchData[i].selector
          };
        }
      });
    }
  }
})();
