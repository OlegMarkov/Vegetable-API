<template>
<v-row justify="center">
    <!-- <v-avatar color="blue" :size="size" @click.stop="dialog = true">
         <v-img v-if="isImageLoaded" :src="avatar"></v-img>
         <span v-else class="white--text headline">{{initials}}</span>
        
    </v-avatar> -->

     <v-badge
        bordered
        bottom
        icon="mdi-camera"
        offset-x="10"
        offset-y="10"
        @click.native="dialog = true"
      >
        <v-avatar :color="color" :style="[isShowAvatar ? {'border-style': 'solid'} : {}]" :size="size" >
          <v-img v-if="isShowAvatar" :src="currentAvatar"></v-img>
          <span v-else class="white--text headline">{{initials}}</span>
        </v-avatar>
      </v-badge>

    <v-dialog v-model="dialog" width="600">
        <v-card class="aligner text-center" color="white" min-height="500">

            <v-card-text v-if="!isImageLoaded || loading">
                <!-- <v-btn @click="$refs.file.click()">
                    <input type="file" ref="file" @change="uploadImage($event)" accept="image/*" v-show="false">
                    Browse File
                </v-btn> -->
                <v-btn :loading="loading" :disabled="loading" color="blue-grey" class="ma-2 white--text" @click="$refs.file.click()">
                    <input type="file" ref="file" @change="uploadImage($event)" accept="image/*" v-show="false">
                    Browse File
                    <v-icon right dark>mdi-cloud-upload</v-icon>
                </v-btn>
            </v-card-text>
            <v-card-text v-else>

                <v-list>
                    <v-list-item>
                        <v-list-item-avatar :size="size">
                            <v-img :src="avatar"></v-img>
                        </v-list-item-avatar>
                        <v-spacer></v-spacer>
                        <v-list-item-content>
                            <v-row>
                                <v-btn class="ma-2" tile outlined color="success" @click="clearImage()">
                                    <v-icon left>mdi-image-off</v-icon> Clear
                                </v-btn>

                                <v-btn class="ma-2" tile color="success" @click="saveImage()">
                                    <v-icon left>mdi-upload</v-icon> Save
                                </v-btn>
                            </v-row>

                        </v-list-item-content>
                    </v-list-item>
                </v-list>

                <Cropper classname="upload-example-cropper" v-if="isImageLoaded" :stencilComponent="$options.components.CircleStencil" :src="image" @change="onChange" @ready="onImageReady"/>
            </v-card-text>
        </v-card>
        <v-btn @click="dialog=false">Close</v-btn>
    </v-dialog>
</v-row>
</template>

<script>
import {
    Cropper,
    CircleStencil
} from 'vue-advanced-cropper'

export default {
    data() {
        return {
            dialog: false,
            image: null,
            isImageLoaded: false,
            avatar: {},
            loader: null,
            loading: false,
            extension: ''
        }
    },
    methods: {
        uploadImage(event) {
            // Reference to the DOM input element
            var input = event.target;
            this.loading = true;
            // Ensure that you have a file before attempting to read it
            if (input.files && input.files[0]) {
                // create a new FileReader to read this image and convert to base64 format
                var reader = new FileReader();
                // Define a callback function to run, when FileReader finishes its job
                reader.onload = (e) => {
                    // Note: arrow function used here, so that "this.imageData" refers to the imageData of Vue component
                    // Read image as base64 and set to imageData
                    this.isImageLoaded = true;
                    this.loading = false;
                    this.image = e.target.result;   
                    this.extension = input.files[0].name.split('.').pop();                 
                }
                // Start the reader job - read file as a data url (base64 format)
                reader.readAsDataURL(input.files[0]);
            }
        },
         saveImage() {
             this.$emit('savedImage', {avatar: this.avatar, extension: this.extension});            
             this.dialog = false;
         },
         onImageReady(){
             this.isImageLoaded = true;
             this.loading = false;
         },
        clearImage() {
            this.image = null;
            this.isImageLoaded = false;
            this.loading = false;
        },
        onChange({
            coordinates,
            canvas
        }) {
            this.coordinates = coordinates
            // You able to do different manipulations at a canvas
            // but there we just get a cropped image
            this.avatar = canvas.toDataURL()
        }
    },
    computed: {
        currentAvatar(){
            if(this.currentImage) return this.currentImage;
            return this.avatar;
        },

        isShowAvatar(){
            if(this.isImageLoaded) return this.isImageLoaded;
            return this.currentImage != "";
        }

    },
    components: {
        Cropper,
        CircleStencil
    },
    props: {
        size: String,
        employeeId: String,
        initials: String,
        color: String,
        currentImage: String
    }
}
</script>

<style scoped>
.aligner {
    display: flex;
    align-items: center;
    justify-content: center;
}

.v-dialog>.v-card>.v-card__text {
    padding-top: 24px;
}

.v-list-item {
    position: absolute;
    top: 5px;
}

.v-list-item__content {
    min-width: 300px;
    padding-left: 5px;
}

.upload-example-cropper {
	border: solid 1px #EEE;
	height: 300px;
	width: 100%;
    padding-top: 20px;
}

</style>
