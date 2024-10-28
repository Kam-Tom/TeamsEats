import { Status } from "./Status";
import { ItemData } from "./ItemData";

export interface DetailsData {
    id: number;
    isOwner: boolean;
    authorName: string;
    authorPhoto: string;
    restaurant: string;
    phoneNumber: string;
    bankAccount: string;
    minimalPrice: number;
    deliveryCost: number;
    minimalPriceForFreeDelivery: number;
    items: ItemData[];
    status: Status;
    closingTime: Date;
}
