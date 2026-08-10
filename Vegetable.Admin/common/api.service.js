import Vue from "vue";
import axios from "axios";
import VueAxios from "vue-axios";

import Constants from "~/config.js";

const ApiService = {
    init() {
      Vue.use(VueAxios, axios);
      Vue.axios.defaults.baseURL = Constants.ApiBaseUrl;
    },
  
  
    query(resource, params) {
      return Vue.axios.get(resource, params).catch(error => {
        throw new Error(`[OBS] ApiService ${error}`);
      });
    },
  
    get(resource, guid = "") {
      return Vue.axios.get(`${resource}/${guid}`).catch(error => {
        throw new Error(`[OBS] ApiService ${error}`);
      });
    },
  
    post(resource, params) {
      return Vue.axios.post(`${resource}`, params);
    },
  
    update(resource, guid, data) {
      return Vue.axios.put(`${resource}/${guid}`, data);
    },
  
    put(resource, params) {
      return Vue.axios.put(`${resource}`, params);
    },
  
    delete(resource, guid) {
      return Vue.axios.delete(`${resource}/${guid}`).catch(error => {
        throw new Error(`[OBS] ApiService ${error}`);
      });
    }
  };

  export default ApiService;

export const OwnerService = {
  get(guid) {
    return ApiService.get("owner", guid);
  },
  create(params) {
    return ApiService.post("owner", { owner: params });
  },
  update(guid, params) {
    return ApiService.update("owner", guid, { owner: params });
  },
  destroy(guid) {
    return ApiService.delete(`articles/${guid}`);
  }
};

export const ServicesService = {
    get(guid) {
      return ApiService.get("owner", `${guid}/services`);
    },

    fetch() {
      return ApiService.get("owner/service");
    },

    create(service) {
      return ApiService.post("owner/service", { ...service });
    },   

    update(serviceId, service) {
      return ApiService.update("owner/service", serviceId, { ...service });
    }, 
  
    delete(serviceId) {
      return ApiService.delete(`owner/service`, serviceId);
    }
  };

  export const EmployeesService = {
    get(guid) {
      return ApiService.get("owner", `${guid}/employees`);
    },

    fetch() {
      return ApiService.get("owner/employee");
    },    

    create(employee) {
      return ApiService.post("owner/employee", { ...employee });
    },   

    update(employeeId, employee) {
      return ApiService.update("owner/employee", employeeId, { ...employee });
    }, 
  
    delete(employeeId) {
      return ApiService.delete(`owner/employee`, employeeId);
    }
  };

  export const SettingsService = {
    getCurrencies() {
      return ApiService.get("settings/currency");
    }    
  };

  export const ImagesService = {
    add(imageInfo) {
      return ApiService.post("images", imageInfo);
    }    
  };
