<template>
<v-dialog v-model="dialog" fullscreen transition="dialog-bottom-transition" content-class="general-settings" @input="addScrollHandler">
    <template v-slot:activator="{ on }">
        <v-list dense class>
            <v-list-item link v-on="on">
                <v-list-item-action>
                    <v-icon>settings</v-icon>
                </v-list-item-action>
                <v-list-item-content>
                    <v-list-item-title class>{{$t('general-settings.title')}}</v-list-item-title>
                </v-list-item-content>
            </v-list-item>
        </v-list>
    </template>

    <div class="text-center">
        <v-snackbar v-model="snackbar" top>
            {{$t('general-settings.snackbar-saved')}}
            <v-btn text @click="snackbar = false"> {{$t('general-settings.snackbar-close')}}</v-btn>
        </v-snackbar>
    </div>

    <v-form ref="form" v-model="valid" :lazy-validation="false">
        <v-card>
            <v-toolbar color="white">
                <v-btn icon @click="closeDialog()">
                    <v-icon>keyboard_backspace</v-icon>
                </v-btn>
                <v-toolbar-title>{{$t('general-settings.title')}}</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-toolbar-items>
                    <v-tooltip left v-if="!valid" color="red">
                        <template v-slot:activator="{ on }">
                            <div v-on="on" class="my-4">
                                <v-btn disabled v-on="on" text>{{$t('general-settings.save')}}</v-btn>
                            </div>
                        </template>
                        <span>{{$t('general-settings.validation-summary')}}</span>
                    </v-tooltip>
                    <div v-else class="my-4">
                        <v-btn text @click="saveGeneralSettings()">{{$t('general-settings.save')}}</v-btn>
                    </div>
                </v-toolbar-items>
            </v-toolbar>

            <v-container>
                <v-row>
                    <v-col cols="12" lg="2" md="4">
                        <div class="vegetable-menu">
                            <div>
                                <v-btn text small @click="$vuetify.goTo('#company',{ container: '.general-settings', offset: 25  })">{{$t('general-settings.company-information')}}</v-btn>
                            </div>
                            <div>
                                <v-btn text small @click="$vuetify.goTo('#address',{ container: '.general-settings', offset: 25 })">{{$t('general-settings.addresses')}}</v-btn>
                            </div>
                            <div>
                                <v-btn text small @click="$vuetify.goTo('#account',{ container: '.general-settings', offset: 25  })">{{$t('general-settings.account')}}</v-btn>
                            </div>
                            <div>
                                <v-btn text small @click="$vuetify.goTo('#subscription',{ container: '.general-settings', offset: 25  })">{{$t('general-settings.subscription')}}</v-btn>
                            </div>
                        </div>
                        <div></div>
                    </v-col>
                    <v-col cols="12" lg="10" md="8">
                        <v-list dense>
                            <v-subheader class="title">
                                <a id="company">
                                    <h3>{{$t('general-settings.company-information')}}</h3>
                                </a>
                            </v-subheader>

                            <v-list-item>
                                <v-list-item-content>
                                    <v-row no-gutters>
                                        <v-col cols="2" align-self="center" class="text-left">
                                            <span>{{ $t('general-settings.company-title') }}</span>
                                        </v-col>
                                        <v-col cols="10">
                                            <v-text-field v-model="owner.title" :counter="50" required :rules="[required(), maxLength(50)]"></v-text-field>
                                            <small>{{ $t('general-settings.company-title-description') }}</small>
                                        </v-col>
                                    </v-row>
                                </v-list-item-content>
                            </v-list-item>

                            <v-list-item>
                                <v-list-item-content>
                                    <v-row no-gutters>
                                        <v-col cols="2" align-self="center" class="text-left">
                                            <span>{{ $t('general-settings.company-description') }}</span>
                                        </v-col>
                                        <v-col cols="10">
                                            <v-text-field v-model="owner.description" :counter="500" :rules="[maxLength(500)]"></v-text-field>
                                            <small>{{ $t('general-settings.company-description-description') }}</small>
                                        </v-col>
                                    </v-row>
                                </v-list-item-content>
                            </v-list-item>

                            <v-list-item>
                                <v-list-item-content>
                                    <v-row no-gutters>
                                        <v-col cols="2" align-self="center" class="text-left">
                                            <span>{{ $t('general-settings.alias') }}</span>
                                        </v-col>
                                        <v-col cols="10">
                                            <v-text-field v-model="owner.alias" :counter="50" required :rules="[required(), maxLength(50)]"></v-text-field>
                                            <small>{{ $t('general-settings.alias-description') }}</small>
                                        </v-col>
                                    </v-row>
                                </v-list-item-content>
                            </v-list-item>
                            <v-list-item>
                                <v-list-item-content>
                                    <v-row no-gutters>
                                        <v-col cols="2" align-self="center" class="text-left">
                                            <span>{{ $t('services.service-duration') }}</span>
                                        </v-col>
                                        <v-col cols="10">
                                            <v-select required v-model="owner.currency" item-text="name" item-value="id" :items="currencies" :rules="[required()]" return-object></v-select>
                                            <small>{{ $t('services.service-duration-description') }}</small>
                                        </v-col>
                                    </v-row>
                                </v-list-item-content>
                            </v-list-item>

                            <v-list-item>
                                <v-list-item-content>
                                    <v-row no-gutters>
                                        <v-col cols="2" align-self="center" class="text-left">
                                            <span>{{$t('general-settings.turn-on-personal-web-site')}}</span>
                                        </v-col>
                                        <v-col cols="10">
                                            <v-switch color="primary" align-self="center" v-model="owner.allowSite"></v-switch>
                                            <small>{{$t('general-settings.turn-on-personal-web-site-description')}}</small>
                                        </v-col>
                                    </v-row>
                                </v-list-item-content>
                            </v-list-item>

                            <v-list-item>
                                <v-list-item-content>
                                    <v-row no-gutters>
                                        <v-col cols="2" align-self="center" class="text-left">
                                            <span>{{$t('general-settings.personal-web-site')}}</span>
                                        </v-col>
                                        <v-col cols="10">
                                            <v-text-field readonly :value="pwsUrl"></v-text-field>
                                            <small>{{$t('general-settings.personal-web-site-description')}}</small>
                                        </v-col>
                                    </v-row>
                                </v-list-item-content>
                            </v-list-item>

                            <v-list-item>
                                <v-list-item-content>
                                    <v-row no-gutters>
                                        <v-col cols="2" align-self="center" class="text-left">
                                            <span>{{$t('general-settings.invite-link')}}</span>
                                        </v-col>
                                        <v-col cols="10">
                                            <v-text-field readonly :value="inviteLink"></v-text-field>
                                            <small>{{$t('general-settings.invite-link-description')}}</small>
                                        </v-col>
                                    </v-row>
                                </v-list-item-content>
                            </v-list-item>
                        </v-list>
                        <v-divider></v-divider>
                        <v-list dense>
                            <v-subheader class="title">
                                <a id="address">
                                    <h4>{{$t('general-settings.addresses')}}</h4>
                                </a>
                            </v-subheader>
                            <div v-for="(address,i) in owner.addresses" :key="i">
                                <v-list-item>
                                    <v-list-item-content>
                                        <v-row no-gutters>
                                            <v-col cols="2" align-self="center" class="text-left">
                                                <span>{{address.description}}</span>
                                            </v-col>
                                            <v-col cols="10">
                                                <v-btn text small @click="$set(address, 'expand', !address.expand)" class>
                                                    <span>{{$t('general-settings.address-edit')}}</span>
                                                </v-btn>
                                                <v-btn text small @click.stop="deleteAddress(address)" class color="red">
                                                    <span>{{$t('general-settings.address-delete')}}</span>
                                                </v-btn>
                                            </v-col>
                                        </v-row>
                                    </v-list-item-content>
                                </v-list-item>
                                <v-expand-transition>
                                    <v-row no-gutters class="ml-10" v-if="Boolean(address.expand)">
                                        <v-container>
                                            <v-row>
                                                <v-col cols="12" md="6">
                                                    <v-text-field v-model="address.description" :counter="50" :label="$t('general-settings.address-description')" required :rules="[required(), maxLength(50)]"></v-text-field>
                                                </v-col>

                                                <v-col cols="12" md="6">
                                                    <v-text-field v-model="address.state" :counter="50" :rules="[maxLength(50)]" :label="$t('general-settings.address-state')"></v-text-field>
                                                </v-col>

                                                <v-col cols="12" md="6">
                                                    <v-text-field v-model="address.city" :counter="30" :rules="[maxLength(30)]" :label="$t('general-settings.address-city')"></v-text-field>
                                                </v-col>

                                                <v-col cols="12" md="6">
                                                    <v-text-field v-model="address.postalCode" :counter="10" :rules="[maxLength(10)]" :label="$t('general-settings.address-postalcode')"></v-text-field>
                                                </v-col>

                                                <v-col cols="12" md="6">
                                                    <v-text-field v-model="address.street" :counter="50" :rules="[maxLength(50)]" :label="$t('general-settings.address-street')"></v-text-field>
                                                </v-col>

                                                <v-col cols="12" md="6">
                                                    <v-text-field v-model="address.unit" :counter="50" :rules="[maxLength(50)]" :label="$t('general-settings.address-unit')"></v-text-field>
                                                </v-col>
                                            </v-row>
                                        </v-container>
                                    </v-row>
                                </v-expand-transition>
                            </div>

                            <v-btn text small @click.stop="addAddress()" class="mt-4 mb-4">
                                <v-icon>add</v-icon>
                                <span>{{$t('general-settings.address-add')}}</span>
                            </v-btn>
                        </v-list>
                        <v-divider></v-divider>
                        <v-list three-line subheader>
                            <v-subheader class="title">
                                <a id="account">
                                    <h4>{{$t('general-settings.account')}}</h4>
                                </a>
                            </v-subheader>

                            <v-list-item>
                                <v-list-item-content>
                                    <v-row no-gutters>
                                        <v-col cols="2" align-self="center" class="text-left">
                                            <span>{{$t('general-settings.account-title')}}</span>
                                        </v-col>
                                        <v-col cols="10">
                                            <v-avatar size="30">
                                                <v-img :src="$store.state.owner.user.picture"></v-img>
                                            </v-avatar>
                                            <span>{{$store.state.owner.user.email}}</span>

                                            <v-btn text small @click.stop="logout">
                                                <v-icon>exit_to_app</v-icon>
                                                <span>{{ $t('general-settings.logout') }}</span>
                                            </v-btn>
                                        </v-col>
                                    </v-row>
                                </v-list-item-content>
                            </v-list-item>
                        </v-list>
                        <v-divider></v-divider>
                        <v-list three-line subheader>
                            <v-subheader class="title">
                                <a id="subscription">
                                    <h4>{{$t('general-settings.subscription')}}</h4>
                                </a>
                            </v-subheader>
                        </v-list>
                    </v-col>
                </v-row>
            </v-container>
        </v-card>
    </v-form>
</v-dialog>
</template>

<script>
import {
    mapGetters
} from "vuex";
import axios from "axios";
import _ from "underscore";
import Constants from "~/config.js";

import {
    FETCH_CURRENCIES
} from "@/store/actions.type";

export default {
    data: function () {
        return {
            valid: true,
            dialog: false,
            snackbar: false,
            owner: JSON.parse(JSON.stringify(this.$store.state.owner.owner)),
            inviteLink: Constants.AdminBaseUrl + "login?companyid=" + this.$store.state.owner.id
        };
    },
    computed: {
        pwsUrl: function () {
            return Constants.ObsBaseUrl + this.owner.alias;
        },
        ...mapGetters(["currencies"])
    },
    created: function () {
        this.populateCurrencies();
    },
    methods: {
        closeDialog() {
            this.dialog = false;
            this.owner = JSON.parse(JSON.stringify(this.$store.state.owner.owner));
        },
        saveGeneralSettings() {
            // console.log(this.owner.alias);
            axios.put(Constants.ApiOwnerUrl, this.owner).then(response => {
                this.$store.commit("setOwner", this.owner);
                this.snackbar = true;
            });
        },
        required() {
            return value => !!value || this.$t("validation.required");
        },
        maxLength(max) {
            return value =>
                (value || "").length <= max || this.$t("validation.maxLength");
        },
        addAddress() {
            var address = {
                description: "New Address",
                state: "",
                city: "",
                postalCode: "",
                street: "",
                unit: "",
                expand: true
            };
            this.owner.addresses.push(address);
        },
        deleteAddress(address) {
            var addressIndex = this.owner.addresses.indexOf(address);
            this.owner.addresses.splice(addressIndex, 1);
        },
        populateCurrencies() {
            this.$store.dispatch(FETCH_CURRENCIES)
        },
        addScrollHandler() {
            // document
            //   .getElementsByClassName("general-settings")[0]
            //   .removeEventListener("scroll", this.handleScroll);
            // document
            //   .getElementsByClassName("general-settings")[0]
            //   .addEventListener("scroll", this.handleScroll);
        },
        handleScroll() {
            // var address = document.getElementById("address");
            // var addressTop = address.getBoundingClientRect().top;
            //   if (addressTop == 0) {
            //     console.log("address top");
            //   }
        }
    }
};
</script>

<style scoped>
.vegetable-menu {
    position: -webkit-sticky;
    position: sticky;
    top: 90px;
}

.general-settings>.v-card>.v-toolbar {
    position: -webkit-sticky;
    position: sticky;
    top: 0px;
    z-index: 100;
}
</style>
