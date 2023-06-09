import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MyInputModule } from '@shared/modules/my-input/my-input.module';
import { CategoryComponent } from '@modules/admin/pages/category/category.component';
import { SalesPointComponent } from '@modules/admin/pages/sales-point/sales-point.component';
import { CategoryFormHandlerService } from '@modules/admin/pages/category/services/category-form-handler.service';
import { CategoryAddComponent } from '@modules/admin/pages/category/components/category-add/category-add.component';
import { SalesPointFormHandlerService } from '@modules/admin/pages/sales-point/services/sales-point-form-handler.service';
import { CategoryModifyComponent } from '@modules/admin/pages/category/components/category-modify/category-modify.component';
import { SalesPointAddComponent } from '@modules/admin/pages/sales-point/components/sales-point-add/sales-point-add.component';
import { SalesPointModifyComponent } from '@modules/admin/pages/sales-point/components/sales-point-modify/sales-point-modify.component';
import { CategorySetActionComponent } from '@modules/admin/pages/category/components/category-set-action/category-set-action.component';
import { CategoryTypeSwitchComponent } from '@modules/admin/pages/category/components/category-type-switch/category-type-switch.component';
import { SalesPointSetActionComponent } from '@modules/admin/pages/sales-point/components/sales-point-set-action/sales-point-set-action.component';
import { SalesPointTypeSwitchComponent } from '@modules/admin/pages/sales-point/components/sales-point-type-switch/sales-point-type-switch.component';
import { ProductFormHandlerService } from './product/services/product-form-handler.service';
import { ProductAddComponent } from './product/components/product-add/product-add.component';
import { ProductModifyComponent } from './product/components/product-modify/product-modify.component';
import { ProductSetActionComponent } from './product/components/product-set-action/product-set-action.component';
import { ProductTypeSwitchComponent } from './product/components/product-type-switch/product-type-switch.component';
import { ProductComponent } from './product/product.component';
import { LzNestedDropdownModule } from '@shared/modules/lz-nested-dropdown/lz-nested-dropdown.module';
import { TopMenuService } from '@modules/top-menu/api/top-menu.service';
import { OfferAddComponent } from './offer/components/offer-add/offer-add.component';
import { OfferModifyComponent } from './offer/components/offer-modify/offer-modify.component';
import { OfferSetActionComponent } from './offer/components/offer-set-action/offer-set-action.component';
import { OfferTypeSwitchComponent } from './offer/components/offer-type-switch/offer-type-switch.component';
import { OfferComponent } from './offer/offer.component';
import { OfferFormHandlerService } from './offer/services/offer-form-handler.service';
import { ListItemsModule } from '@shared/modules/list-items/list-items.module';
import { Base64Module } from '@shared/modules/base64/base64.module';
import { UserSetActionComponent } from './user/components/user-set-action/user-set-action.component';
import { UserTypeSwitchComponent } from './user/components/user-type-switch/user-type-switch.component';
import { UserFormHandlerService } from './user/services/user-form-handler.service';
import { UserComponent } from './user/user.component';
import { UserBanComponent } from './user/components/user-ban/user-ban.component';
import { UserUnbanComponent } from './user/components/user-unban/user-unban.component';

@NgModule({
  declarations: [
    CategoryComponent,
    SalesPointComponent,
    CategoryAddComponent,
    SalesPointAddComponent,
    CategoryModifyComponent,
    SalesPointModifyComponent,
    CategorySetActionComponent,
    CategoryTypeSwitchComponent,
    SalesPointSetActionComponent,
    SalesPointTypeSwitchComponent,
    ProductComponent,
    ProductAddComponent,
    ProductModifyComponent,
    ProductSetActionComponent,
    ProductTypeSwitchComponent,
    UserComponent,
    UserBanComponent,
    UserUnbanComponent,
    UserSetActionComponent,
    UserTypeSwitchComponent,
    OfferComponent,
    OfferAddComponent,
    OfferModifyComponent,
    OfferSetActionComponent,
    OfferTypeSwitchComponent,
  ],
  imports: [
    SharedModule,
    CommonModule,
    MatIconModule,
    MyInputModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCheckboxModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatSlideToggleModule,
    MatButtonToggleModule,
    LzNestedDropdownModule,
    ListItemsModule,
    Base64Module,
  ],
  providers: [
    ProductFormHandlerService,
    CategoryFormHandlerService,
    SalesPointFormHandlerService,
    UserFormHandlerService,
    OfferFormHandlerService,
    TopMenuService,
  ]
})
export class AdminPagesModule { }
