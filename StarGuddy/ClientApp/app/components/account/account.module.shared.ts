﻿import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";

import { AccountService } from "./Account.Service";
import { AccountLoginComponent } from "./login/login.component";
import { SignUpComponent } from "./signup/signup.component";
import { SignUpJobProviderComponent } from "./signup/jobProvider/jobProvider.component";
import { SignUpJobSeekerComponent } from "./signup/jobSeeker/jobSeeker.component";

//Account Management
import { ChangePwdComponent } from "./management/changePassword/changePwd.component";

@NgModule({
    declarations: [
        AccountLoginComponent,
        SignUpComponent,
        SignUpJobProviderComponent,
        SignUpJobSeekerComponent,
        ChangePwdComponent
    ],
    imports: [
        RouterModule,
        CommonModule,
        FormsModule,
        RouterModule.forRoot([
            { path: "login", component: AccountLoginComponent },
            { path: "signup", component: SignUpComponent },
            { path: "jobseeker", component: SignUpJobSeekerComponent },
            { path: "jobprovider", component: SignUpJobProviderComponent },
            { path: "changePwd", component: ChangePwdComponent }
        ])
    ],
    providers: [AccountService],
    exports: [
        AccountLoginComponent,
        SignUpComponent,
        SignUpJobProviderComponent,
        SignUpJobSeekerComponent,
        ChangePwdComponent
    ]
})

export class AccountModuleShared {
}