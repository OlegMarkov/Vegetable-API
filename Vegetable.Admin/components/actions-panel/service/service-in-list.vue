<template>
<v-tooltip bottom="">
    <template v-slot:activator="{ on }">
        <v-list-item link @mouseover="mousehover = true" @mouseleave="mousehover = false" v-on="on">
            <v-list-item-icon>
                <v-icon :color="serviceInfo.service.color">mdi-circle</v-icon>
            </v-list-item-icon>
            <v-list-item-content>
                <v-list-item-title>{{ serviceInfo.service.title }}</v-list-item-title>
            </v-list-item-content>

            <v-list-item-action v-show="mousehover">
                <service-edit-view :serviceId="serviceInfo.id" :editMode="true" />
            </v-list-item-action>
            <v-list-item-action v-show="mousehover">
                <v-btn title="Delete" text icon @click="deleteService">
                    <v-icon small>delete</v-icon>
                </v-btn>
            </v-list-item-action>
        </v-list-item>
    </template>
    <span>{{serviceInfo.service.title}}</span>
</v-tooltip>
</template>

<script>
import {
    DELETE_SERVICE
} from "@/store/actions.type";
import ServiceEditView from '~/components/actions-panel/service/service-edit.vue'
export default {
    data() {
        return {
            mousehover: false
        }
    },
    components: {
        ServiceEditView
    },
    computed: {
        serviceInfo() {
            return this.currentService
        }
    },
    methods: {
        deleteService() {
            this.$store.dispatch(DELETE_SERVICE, this.serviceInfo.id)
        }
    },
    props: ['currentService']
}
</script>

<style scoped>
.v-list-item__action {
    margin: 0;
}

.v-v-list-item__title {
    margin-left: 15px;
}

.v-list-item__action:last-of-type:not(:only-child) {
    margin: 0;
}
</style>
