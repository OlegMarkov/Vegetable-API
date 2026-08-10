import Vue from "vue";
import Vuex from "vuex";
import createPersistedState from 'vuex-persistedstate'
import ApiService from "@/common/api.service";

import owner from "./owner.module";
import service from "./service.module";
import employee from "./employee.module";
import settiings from "./settings.module";


Vue.use(Vuex);

ApiService.init();

const store = () => {
  return new Vuex.Store({
  
  modules: {
    owner, service, settiings, employee
  },
  plugins: [createPersistedState({ key: 'vegetable' })]
})
};
export default store
