import _ from 'underscore'

import {
  UPDATE_OWNER
} from "./actions.type";

import {
  SET_SERVICES
} from "./mutations.type";


export const state = {
  locales: ['en', 'ru'],
  locale: 'en',
  owner: null,
  selectedEmployees: null,
  selectedServices: null,
  normalizedData: null,
  denormalizedData: null,
  authenticated: null,
  tempCompanyId: null,
  user: {}
};

// export const actions = {
//   async [UPDATE_OWNER](context, owner) {
//       const { data } = await ServicesService.get(ownerId);
//       context.commit(SET_SERVICES, data.services);
//       return data.services;
//   },


// };

export const mutations = {
  SET_LANG(state, locale) {
    if (state.locales.indexOf(locale) !== -1) {
      state.locale = locale
    }
  },
  setOwner(state, owner) {
    state.owner = owner
  },
  setSelectedEmployees(state, selectedEmployees) {
    state.selectedEmployees = selectedEmployees
  },
  setSelectedServices(state, selectedServices) {
    state.selectedServices = selectedServices
  },
  updateEmployee(state, employee) {
    let employees = state.owner.employees
    let index = _.findIndex(employees, (emp) => emp.id === employee.id)
    employees[index] = { ...employee }
    state.owner.employees = [...employees]
  },

  setNormalizedData(state, normalizedData) {
    state.normalizedData = normalizedData
  },
  setDenormalizedData(state, denormalizedData) {
    state.denormalizedData = denormalizedData
  },
  setAuthenticated(state, authenticated) {
    state.authenticated = authenticated
  },
  setTempCompanyId(state, companyId) {
    state.tempCompanyId = companyId
  },
  setUser(state, user) {
    state.user = user
  }
};

const getters = {
  currentOwner: state => state.owner,
  addresses: state => state.owner.addresses
};


export default {
  state,
  mutations,
  getters
};
