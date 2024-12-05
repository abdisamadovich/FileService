import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutComponent } from './components/layout/layout.component';
import { HomeComponent } from './components/home/home.component';

const routes: Routes = [{
    path: "",
    component: LayoutComponent,
    children: [
        {
            path: "home",
            component: HomeComponent
        },
        {
            path: "",
            redirectTo: "/home",
            pathMatch: "full"
        },
        { 
            path: '**', 
            component: HomeComponent 
        }
    ]
}];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
