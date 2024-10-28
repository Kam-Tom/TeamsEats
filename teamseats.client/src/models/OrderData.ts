import { Status } from "./Status";

export interface OrderData {
    id: number;
    isOwner: boolean;
    isParticipating: boolean;
    authorName: string;
    authorPhoto: string;
    deliveryCost: number;
    restaurant: string;
    status: Status;
    closingTime: Date;
}
