import Vuex from 'vuex'

const store = () => {
  return new Vuex.Store({
    state: {
      locales: ['en', 'ru'],
      locale: 'en',
      owner: null,
      steps: null,
      selectedAddress: null,
      selectedService: null,
      selectedEmployee: null,
      selectedDate: null,
      selectClient: null
    },
    mutations: {
      changeLocale(state, locale) {
        if (state.locales.indexOf(locale) !== -1) {
          state.locale = locale
          this.app.i18n.locale = locale;
        }
      },
      setOwner(state, owner){
        state.owner = owner;
      },
      setSteps(state, steps){
        state.steps = steps;
      },
      changeAddress(state, address) {
        state.selectedAddress = address;
      },
      changeService(state, service) {
        state.selectedService = service;
      },
      changeEmployee(state, employee) {
        state.selectedEmployee = employee;
      },
      changeDate(state, date) {
        state.selectedDate = date;
      },
      changeTime(state, time) {
        state.selectedTime = time;
      },
      changeDateTime(state, dateTime) {
        state.selectedDateTime = dateTime;
      },
      changeClient(state, client) {
        state.selectedClient = client;
      }
    },
    getters: {
      getSelectedAddress: state => {
        return state.selectedAddress
      },
      getSelectedService: state => {
        return state.selectedService
      },
      getSelectedEmployee: state => {
        return state.selectedEmployee
      },
      getSelectedLocale: state => {
        return state.locale
      }
    }
  })
}

export default store
