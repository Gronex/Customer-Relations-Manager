(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Companies', Companies);

  Companies.$inject = ['companies', '$state'];

  /* @ngInject */
  function Companies(companies, $state) {
    var vm = this;
    vm.query = {};
    vm.itemCount = 0;
    vm.headers = [
      {
        label: "Name",
        selector: "name",
        type: "link",
        link: "Companies.view"
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
        selector: "webSite",
        type: "ext-link"
      },
      {
        icon: "fa fa-pencil-square-o",
        type: "btn-link",
        link: "Companies.edit"
      }
    ];

    vm.getCompanies = getCompanies;

    activate();

    function activate() {
      setupData(companies);
    }

    function getCompanies(sortParam) {
      _.merge(vm.query, {orderBy: sortParam});
      $state.go("Companies.list", vm.query);
    }

    function setupData(data) {
      vm.itemCount = data.itemCount;
      vm.companies = data.data;
      _.merge(vm.query, {
        pageSize: data.pageSize,
        page: data.pageNumber,
        orderBy: data.query.orderBy
      });
    }
  }
})();



