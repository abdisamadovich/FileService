import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

//For (ngModel,ngForm ...)
import { FormsModule } from '@angular/forms';

//For (ngIf,ngFor ...)
import { CommonModule } from '@angular/common';

//Router
import { AppRoutingModule } from './app-routing.module';

//Main Component
import { AppComponent } from './app.component';

//Components I created
import { FooterComponent } from './components/footer/footer.component';
import { HomeComponent } from './components/home/home.component';
import { HeaderComponent } from './components/header/header.component';
import { LayoutComponent } from './components/layout/layout.component';

//Toastr
import { ToastrModule } from 'ngx-toastr';

//Kendo Ui
import { GridModule } from '@progress/kendo-angular-grid';
import { NavigationModule } from "@progress/kendo-angular-navigation";
import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { DialogsModule } from "@progress/kendo-angular-dialog";
import { SVGIconModule } from '@progress/kendo-angular-icons';
import { InputsModule } from "@progress/kendo-angular-inputs";
import { PagerModule } from "@progress/kendo-angular-pager";
import { LabelModule } from '@progress/kendo-angular-label';
import { IntlModule } from "@progress/kendo-angular-intl";
import { IndicatorsModule } from "@progress/kendo-angular-indicators";
import { UploadsModule } from '@progress/kendo-angular-upload';

@NgModule({
    declarations: [
        //Main Component
        AppComponent,
        //Components I created
        FooterComponent,
        HomeComponent,
        HeaderComponent,
        LayoutComponent
    ],
    imports: [
        HttpClientModule,
        BrowserAnimationsModule,    
        BrowserModule,
        
        //Router
        AppRoutingModule,
        //For (ngModel,ngForm ...)
        FormsModule, 
        //For (ngIf,ngFor ...)
        CommonModule,     
        //Kendo
        NavigationModule,
        PagerModule, 
        LabelModule,
        ButtonsModule, 
        SVGIconModule, 
        DialogsModule, 
        InputsModule, 
        GridModule,
        IndicatorsModule,
        IntlModule,
        UploadsModule,
        //Toastr
        ToastrModule.forRoot({
            timeOut: 2000,
            positionClass: 'toast-top-right',
            preventDuplicates: true,
        })
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
