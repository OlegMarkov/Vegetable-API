import _ from "underscore";

import {
    ServicesService
} from "@/common/api.service";

import {
    CREATE_SERVICE,
    UPDATE_SERVICE,
    DELETE_SERVICE,
    FETCH_SERVICES
} from "./actions.type";

import {
    SET_SERVICES
} from "./mutations.type";

const initialState = {
    services: [],
    selectedServices: []
};

export const state = { ...initialState };

export const actions = {
    async [FETCH_SERVICES](context) {
        const { data } = await ServicesService.fetch();
        context.commit(SET_SERVICES, data);
        return data;
    },

    async [CREATE_SERVICE](context, service) {
        await ServicesService.create(service)
        context.dispatch(FETCH_SERVICES);
    },

    async [UPDATE_SERVICE](context, payload) {
        await ServicesService.update(payload.serviceId, payload.service)
        context.dispatch(FETCH_SERVICES);
    },

    async [DELETE_SERVICE](context, serviceId) {
        await ServicesService.delete(serviceId)
        context.dispatch(FETCH_SERVICES);
    }
};

export const mutations = {
    [SET_SERVICES](state, services) {
        state.services = services;
    }
};

const getters = {
    services: state => state.services,
    getServiceById: state => id => state.services.find(service => service.id === id),
    selectedServices(state) {
        var selectedServices = [];
       
        _.each(state.services, function (service) {
            selectedServices.push({
                id: service.id,
                checked: true,
                service: service
            });
        });

        return selectedServices;
    }
};

export default {
    state,
    actions,
    mutations,
    getters
};