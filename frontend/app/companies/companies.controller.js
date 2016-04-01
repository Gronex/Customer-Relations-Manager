(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Companies', Companies);

  Companies.$inject = ['dataservice'];

  /* @ngInject */
  function Companies(dataservice) {
    var vm = this;
    vm.companies = [];
    vm.pagination = {
      pageSize: 5,
      page: 1
    };
    vm.itemCount = 0;
    vm.headers = [
      {
        label: "Name",
        selector: "name"
      },
      {
        label: "PhoneNumber",
        selector: "phoneNumber"
      },
      {
        label: "Address",
        selector: "address"
      },
      {
        label: "City",
        selector: "city"
      },
      {
        label: "Postal Code",
        selector: "postalCode"
      },
      {
        label: "Country",
        selector: "country"
      },
      {
        label: "Website",
        selector: "website"
      },
      {
        icon: "fa fa-pencil-square-o",
        type: "btn-link",
        link: "Companies.edit"
      }
    ];

    vm.getCompanies = getCompanies;
    vm.sort = sort;

    activate();

    function activate() {
      getCompanies();
    }

    function getCompanies() {
      dataservice.companies
        .get({query: vm.pagination})
        .then(setupData);
    }

    function setupData(data) {
      vm.itemCount = data.itemCount;
      vm.companies = data.data;
    }

    function sort(selector){
      var cpy = angular.copy(vm.pagination);
      cpy.orderBy = selector;
      dataservice.companies
        .get({query: cpy})
        .then(setupData);
    }
  }
})();



