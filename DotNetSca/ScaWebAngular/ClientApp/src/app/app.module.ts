import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
//import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ProjectsComponent } from './projectss/projects/projects.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ProjectViewComponent } from './projectss/project-view/project-view.component';
import { ApiTokensComponent } from './api-tokens/api-tokens.component';
import { ReportViewComponent } from './report-view/report-view.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    ProjectsComponent,
    ProjectViewComponent,
    ApiTokensComponent,
    ReportViewComponent
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    ApiAuthorizationModule,
    RouterModule.forRoot([
      {path: '', component: HomeComponent, pathMatch: 'full'},

      {path: 'projects', component: ProjectsComponent, canActivate: [AuthorizeGuard]},
      {path: 'projects/:id', component: ProjectViewComponent, canActivate: [AuthorizeGuard]},

      {path: 'apiTokens', component: ApiTokensComponent, canActivate: [AuthorizeGuard]},
      {path: 'projects/:projectId/reports/:reportId', component: ReportViewComponent, canActivate: [AuthorizeGuard]},
    ]),
//    NgbModule,
    BrowserAnimationsModule,
    NgbModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
