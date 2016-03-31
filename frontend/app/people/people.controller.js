(function() {
  'use strict';

  angular
      .module('CRM')
      .controller('People', People);

  People.$inject = ['dataservice'];

  /* @ngInject */
  function People(dataservice) {
    var vm = this;

    vm.people = [];
    vm.pagination = {
      pageSize: 5,
      page: 1
    };
    vm.itemCount = 0;
    vm.headers = [
      {
        label: "First name",
        selector: "firstName"
      },
      {
        label: "Last name",
        selector: "lastName"
      },
      {
        label: "Email",
        selector: "email"
      },
      {
        label: "Phone number",
        selector: "phoneNumber"
      },
      {
        icon: "fa fa-pencil-square-o",
        type: "btn-link",
        link: "Person"
      }
    ];

    vm.getPeople = getPeople;
    vm.sort = sort;

    activate();

    function activate() {
      getPeople();
    }

    function getPeople() {
      dataservice.people
        .get({query: vm.pagination})
        .then(setupData);
    }

    function setupData(data) {
      vm.itemCount = data.itemCount;
      vm.people = data.data;
    }

    function sort(selector){
      var cpy = angular.copy(vm.pagination);
      cpy.orderBy = selector;
      dataservice.people
        .get({query: cpy})
        .then(setupData);
    }
  }
})();
