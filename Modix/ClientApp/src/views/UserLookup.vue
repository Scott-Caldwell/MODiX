<template>
    <section class="container section">
        <h1 class="title">User Lookup</h1>
        <div>
            <UserSearch @userSelected="selectedUser = $event" />
        </div>
        <div>
            <UserProfile :user="selectedUser" />
        </div>
    </section>
</template>

<script lang="ts">
import * as _ from 'lodash';
import { Component } from 'vue-property-decorator';
import ModixComponent from '@/components/ModixComponent.vue';
import UserSearch from '@/components/UserLookup/UserSearch.vue';
import UserProfile from '@/components/UserLookup/UserProfile.vue';
import EphemeralUser from '@/models/EphemeralUser';
import UserService from '@/services/UserService';
import store from "@/app/Store";

@Component(
{
    components:
    {
        UserSearch,
        UserProfile,
    }
})
export default class UserLookup extends ModixComponent
{
    selectedUser: EphemeralUser | null = null;

    async mounted()
    {
        const currentUser = store.currentUser();

        if (currentUser)
        {
            this.selectedUser = await UserService.getUserInformation(currentUser.userId);
        }
    }
}
</script>
