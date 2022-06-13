import { Component, OnInit } from '@angular/core';
import {AuthorizeService} from "../../api-authorization/authorize.service";

@Component({
  selector: 'app-api-tokens',
  templateUrl: './api-tokens.component.html',
  styleUrls: ['./api-tokens.component.css']
})
export class  ApiTokensComponent implements OnInit {
  public accessToken? : string | null;

  constructor(public authorize: AuthorizeService) { }

  ngOnInit(): void {
    const me = this;
    this.authorize.getAccessToken()
      .subscribe(x => me.accessToken = x);
  }

}
