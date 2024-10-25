import { Status } from "./Status";
import { ItemData } from "./ItemData";

export interface DetailsData {
    id: number;
    isOwnedByUser: boolean;
    authorName: string;
    authorPhoto: string;
    restaurant: string;
    phoneNumber: string;
    bankAccount: string;
    minimalPrice: number;
    deliveryCost: number;
    minimalPriceForFreeDelivery: number;
    orderItems: ItemData[];
    status: Status;
    closingTime: Date;
}
