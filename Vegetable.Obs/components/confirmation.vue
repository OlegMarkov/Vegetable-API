<style>
  .obs-modal {
    background: #ffffffed !important;
  }

  .obs-modal-title {
    font-size: 25px;
    font-weight: 300;
  }

  .obs-modal-description {
    font-size: 14px;
  }

  .obs-modal-dialog {
    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08) !important;
    border: 1px solid #e5e5e5 !important;
  }

  .obs-error-text {
    font-size: 14px;
  }

  .obs-form-error,
  .obs-form-error:focus {
    border-color: #f0506e;
  }

</style>

<template>
  <li>
    <div>
      <h3 class="uk-text-center">{{ $t('obs.confirmation_title') }}</h3>
      <div class="uk-grid-small" uk-grid>
        <div class="uk-width-1-2@s">
          <input class="uk-input" v-bind:class="{'obs-form-error': errors.some(e => e.field === 'firstName')}" type="text"
            v-bind:placeholder="$t('obs.confirmation_first_name')" v-model="firstName" name="firstName">
          <span class="uk-text-danger obs-error-text" v-show="errors.some(e => e.field === 'firstName')">{{ errors.some(e => e.field === 'firstName') ? errors.find(e => e.field === 'firstName').message : '' }}</span>
        </div>
        <div class="uk-width-1-2@s">
          <input class="uk-input" v-bind:class="{'obs-form-error': errors.some(e => e.field === 'lastName')}" type="text"  v-bind:placeholder="$t('obs.confirmation_last_name')"
            v-model="lastName" name="lastName">
          <span class="uk-text-danger obs-error-text" v-show="errors.some(e => e.field === 'lastName')">{{ errors.some(e => e.field === 'lastName') ? errors.find(e => e.field === 'lastName').message : '' }}</span>
        </div>
        <div class="uk-width-1-2@s">
          <input class="uk-input" v-bind:class="{'obs-form-error': errors.some(e => e.field === 'email')}" type="text"  v-bind:placeholder="$t('obs.confirmation_email')"
            v-model="email" name="email">
          <span class="uk-text-danger obs-error-text" v-show="errors.some(e => e.field === 'email')">{{ errors.some(e => e.field === 'email') ? errors.find(e => e.field === 'email').message : '' }}</span>
        </div>
        <div class="uk-width-1-2@s">
          <input class="uk-input" v-bind:class="{'obs-form-error': errors.some(e => e.field === 'phoneNumber')}" type="text"  v-bind:placeholder="$t('obs.confirmation_phone')"
            v-model="phoneNumber" name="phoneNumber">
          <span class="uk-text-danger obs-error-text" v-show="errors.some(e => e.field === 'phoneNumber')">{{ errors.some(e => e.field === 'phoneNumber') ? errors.find(e => e.field === 'phoneNumber').message : '' }}</span>
        </div>

        <div class="uk-flex uk-flex-center uk-width-1-1">
          <button class="uk-button uk-button-primary" v-on:click.stop.prevent v-on:click="book">{{ $t('obs.confirmation_book') }}</button>
        </div>
      </div>

      <div id="modal-confirmation" class="obs-modal" uk-modal="stack: true; bg-close: false">
        <div class="obs-modal-dialog uk-modal-dialog uk-modal-body">
          <div class="uk-text-center uk-margin-medium-bottom">
            <div class="obs-modal-title">{{ $t('obs.booking_confirmation_title') }}</div>
            <div class="obs-modal-description">{{ $t('obs.booking_confirmation_text', { name: firstName, email: email} ) }}</div>
          </div>

          <div class="obs-modal-description">
            <span class="uk-margin-small-right" uk-icon="icon: check; ratio: 0.8"></span>
            <span v-if="$store.state.selectedService !=  null">{{$store.state.selectedService.title}}</span>
            <br/>
            <span class="uk-margin-small-right" uk-icon="icon: user; ratio: 0.8"></span>
            <span v-if="$store.state.selectedEmployee !=  null">{{$store.state.selectedEmployee.firstName}}</span>
            <br/>
            <span class="uk-margin-small-right" uk-icon="icon: clock; ratio: 0.8"></span>
            <span v-if="$store.state.selectedDate !=  null">{{ $store.state.selectedDateTime }}</span>
            <br />
            <span class="uk-margin-small-right" uk-icon="icon: location; ratio: 0.8"></span>
            <span v-if="$store.state.selectedAddress !=  null">{{$store.state.selectedAddress.state }}</span>

            <p class="uk-text-right">
              <button class="uk-button uk-button-text" type="button" onClick="window.location.reload()">{{ $t('obs.booking_confirmation_exit') }}</button>
            </p>
          </div>
        </div>
      </div>

      <div id="modal-verification" class="obs-modal" uk-modal="stack: true; bg-close: false">
        <div class="obs-modal-dialog uk-modal-dialog">
          <button class="uk-modal-close-default" type="button" uk-close></button>
          <div class="uk-modal-body">
            <div class="uk-text-center uk-margin-medium-bottom">
              <div class="obs-modal-title">{{ $t('obs.confirmation_email_confirmation') }}</div>
              <div class="obs-modal-description">{{ $t('obs.confirmation_verification_code_text') }} {{email}}
                <a class="uk-modal-close">{{ $t('obs.confirmation_change_email') }}</a>
              </div>
            </div>
            <div class="uk-grid-small uk-child-width-expand@s" uk-grid>
              <div>
                <input class="uk-input" type="text"  v-bind:placeholder="$t('obs.confirmation_code_placeholder')" v-model="code">
                <span class="uk-text-danger obs-error-text" v-show="invalidCode">{{ $t('obs.confirmation_code_invalid') }}</span>
              </div>
              <div>
                <button v-on:click="verifyCode" class="uk-button uk-button-primary uk-width-1-1 uk-margin-small-bottom">{{ $t('obs.confirmation_confirmation') }}</button>
              </div>
            </div>
          </div>
          <div class="uk-modal-footer">
            <span v-if="!newEmailAvailable" class="obs-modal-description">{{ $t('obs.confirmation_new_email_text', {seconds: countdown}) }} </span>
            <p v-if="newEmailAvailable" class="uk-text-left">
              <button v-on:click="sendNewCode" class="uk-button uk-button-text" type="button">{{ $t('obs.confirmation_new_code') }}</button>
            </p>
          </div>
        </div>
      </div>


    </div>

  </li>
</template>

<script>
  import axios from 'axios'
  import moment from 'moment'
  let interval = null;

  export default {
    props: ['index'],

    data: function () {
      return {
        errors: [],
        firstName: '',
        lastName: '',
        email: '',
        phoneNumber: '',
        code: '',
        countdown: 10,
        newEmailAvailable: false,
        invalidCode: false
      }
    },
    methods: {
      book() {
        this.code = '';
        this.validateFirstName();
        this.validateLastName();
        this.validateEmail();
        this.validatePhoneNumber();
        if (!this.errors.length) {
            axios.get(process.env.APIBaseURL + 'owner/sendverification/' + this.email);
          UIkit.modal('#modal-verification').show();
          this.startCountdown();
        }
      },
      startCountdown: function () {
        this.resetCountdown();
        this.newEmailAvailable = false;
        this.invalidCode = false;

        interval = setInterval(() => {
          this.countdown -= 1;
          if (this.countdown === 0) {
            this.resetCountdown();
            this.newEmailAvailable = true;
          }
        }, 1000);

      },
      resetCountdown: function () {
        clearInterval(interval);
        this.countdown = 10;
      },
      verifyCode: function () {
        axios.get(process.env.APIBaseURL + 'owner/verifycode/' + this.email + '/' + this.code)
        .then((res) => {
          if (res.data) {
            UIkit.modal('#modal-confirmation').show();
            this.$store.commit('changeClient', this.firstName);
          } else {
            this.invalidCode = true;
          }
        })
      },
      sendNewCode: function () {
        axios.get(process.env.APIBaseURL + 'owner/sendverification/' + this.email);
        this.startCountdown();
        this.code = '';
      },
      validateFirstName: function () {
        if (!this.firstName) {
          if (this.errors.some(e => e.field === 'firstName'))
            return;
          else {
            this.errors.push({
              field: "firstName",
              message: this.$i18n.t('obs.confirmation_first_name_required')
            });
          }
        } else {
          var error = this.errors.find(e => e.field === 'firstName');
          if (error) {
            this.errors.splice(this.errors.indexOf(error), 1);
          }
        }
      },
      validateLastName: function () {
        if (!this.lastName) {
          if (this.errors.some(e => e.field === 'lastName'))
            return;
          else {
            this.errors.push({
              field: "lastName",
              message: this.$i18n.t('obs.confirmation_last_name_required')
            });
          }
        } else {
          var error = this.errors.find(e => e.field === 'lastName');
          if (error) {
            this.errors.splice(this.errors.indexOf(error), 1);
          }
        }
      },
      validateEmail: function () {
        if (!this.email) {
          if (this.errors.some(e => e.field === 'email' && e.type === 'required'))
            return;
          else {
            this.errors.push({
              field: "email",
              type: "required",
              message: this.$i18n.t('obs.confirmation_email_required')
            });
          }
          var error = this.errors.find(e => e.field === 'email' && e.type === 'regex');
          if (error) {
            this.errors.splice(this.errors.indexOf(error), 1);
          }
        } else {
          var error = this.errors.find(e => e.field === 'email' && e.type === 'required');
          if (error) {
            this.errors.splice(this.errors.indexOf(error), 1);
          }
          if (!this.validEmail(this.email)) {
            if (this.errors.some(e => e.field === 'email' && e.type === 'regex'))
              return;
            else {
              this.errors.push({
                field: "email",
                type: "regex",
                message: this.$i18n.t('obs.confirmation_email_invalid')
              });
            }
          } else {
            var error = this.errors.find(e => e.field === 'email' && e.type === 'regex');
            if (error) {
              this.errors.splice(this.errors.indexOf(error), 1);
            }
          }
        }
      },
      validatePhoneNumber: function () {
        if (!this.phoneNumber) {
          if (this.errors.some(e => e.field === 'phoneNumber' && e.type === 'required'))
            return;
          else {
            this.errors.push({
              field: "phoneNumber",
              type: "required",
              message: this.$i18n.t('obs.confirmation_phone_required')
            });
          }
          var error = this.errors.find(e => e.field === 'phoneNumber' && e.type === 'regex');
          if (error) {
            this.errors.splice(this.errors.indexOf(error), 1);
          }
        } else {
          var error = this.errors.find(e => e.field === 'phoneNumber' && e.type === 'required');
          if (error) {
            this.errors.splice(this.errors.indexOf(error), 1);
          }
          if (!this.validPhoneNumber(this.phoneNumber)) {
            if (this.errors.some(e => e.field === 'phoneNumber' && e.type === 'regex'))
              return;
            else {
              this.errors.push({
                field: "phoneNumber",
                type: "regex",
                message: this.$i18n.t('obs.confirmation_phone_invalid')
              });
            }
          } else {
            var error = this.errors.find(e => e.field === 'phoneNumber' && e.type === 'regex');
            if (error) {
              this.errors.splice(this.errors.indexOf(error), 1);
            }
          }
        }
      },
      validEmail: function (email) {
        var re =
          /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
      },
      validPhoneNumber: function (phoneNUmber) {
        var re = /^[0-9]+$/;
        return re.test(phoneNUmber);
      }
    },
    watch: {
      firstName: function () {
        this.validateFirstName();
      },
      lastName: function () {
        this.validateLastName();
      },
      email: function () {
        this.validateEmail();
      },
      phoneNumber: function () {
        this.validatePhoneNumber();
      }
    }

  }

</script>
